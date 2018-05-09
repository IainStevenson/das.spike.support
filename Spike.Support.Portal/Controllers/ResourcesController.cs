using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Spike.Support.Shared.Communication;
using Spike.Support.Shared.Models;

namespace Spike.Support.Portal.Controllers
{
    public class ResourcesController : BaseController
    {
        private readonly ISiteConnector _siteConnector;
        private readonly string _cookieName = "IdentityContextCookie";
        private readonly string _cookieDomain = ".localhost";  // change to real domain for deployment
        private readonly string _defaultIdentity = "anonymous";
        private readonly IIdentityHandler _identityHandler;
        private string _identity;

        public ResourcesController()
        {
            _siteConnector = new SiteConnector();
            _identityHandler = new CookieIdentityHandler(_cookieName, _cookieDomain, _defaultIdentity);
        }

        protected override void Execute(RequestContext requestContext)
        {
            _identity = _identityHandler.GetIdentity(requestContext.HttpContext.Request);
            Debug.WriteLine($"App-Debug: {(nameof(ResourcesController))} {nameof(Execute)} Recieves Identity {_identity}");
            base.Execute(requestContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _identity = _identityHandler.GetIdentity(HttpContext.Request);
            Debug.WriteLine($"App-Debug: {(nameof(ResourcesController))} {nameof(OnActionExecuting)} Recieves Identity {_identity}");
            base.OnActionExecuting(filterContext);
        }

        [Route("resources/{*path}")]
        public async Task<MvcHtmlString> Get(string path)
        {
            Debug.WriteLine($"App-Debug: {(nameof(ResourcesController))} {nameof(Get)} {path}");

            var source = path.Split('/').FirstOrDefault();

            if (string.IsNullOrWhiteSpace(source)) return new MvcHtmlString(string.Empty);

            var ok = Enum.TryParse(source, true, out SupportServices service);

            if (!ok) return await Task.Run(() => new MvcHtmlString(string.Empty));

            _siteConnector.SetCustomHeader("X-Resource", null);

            var resource = await _siteConnector.DownloadView(service, _identity, $"{path}");

            _siteConnector.ClearCustomHeaders("X-Resource");

            return resource;
        }
    }
}