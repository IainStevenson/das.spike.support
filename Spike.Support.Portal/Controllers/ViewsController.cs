using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Portal.Models;
using Spike.Support.Shared.Communication;
using Spike.Support.Shared.Models;

namespace Spike.Support.Portal.Controllers
{
    public class ViewsController : BaseController
    {
        private readonly ISiteConnector _siteConnector;
        private readonly string _cookieName = "IdentityContextCookie";
        private readonly string _cookieDomain = ".localhost";  // change to real domain for deployment
        private readonly string _defaultIdentity = "anonymous";
        private readonly IIdentityHandler _identityHandler;
        private string _identity;
        public ViewsController()
        {
            _siteConnector = new SiteConnector();
            _identityHandler = new CookieIdentityHandler(_cookieName, _cookieDomain, _defaultIdentity);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _identity = "test.User@this.domain"; // simulate a Signed in user   
            Debug.WriteLine($"{(nameof(ViewsController))} {nameof(OnActionExecuting)} Sets Identity {_identity}");

            base.OnActionExecuting(filterContext);
        }

        [Route("")]
        public async Task<ActionResult> Index()
        {
            // OK.... here we go.
            // Make download calls to Users, Accounts, Accounts will ask for a Download roundtrip for Payements.. by Account
            var usersView = await _siteConnector.DownloadView(SupportServices.Users, _identity, "users");
            var accountsView = await _siteConnector.DownloadView(SupportServices.Accounts, _identity, "accounts");
            var indexViewModel = new IndexViewModel
            {
                UsersView = usersView,
                AccountsView = accountsView
            };

            return View("index", indexViewModel);
        }

        [Route("views/{*path}")]
        public async Task<ActionResult> Index(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return new HttpNotFoundResult();
            
            var indexViewModel = new IndexViewModel();

            if (path.ToLower().StartsWith("users".ToLower()))
                indexViewModel.UsersView = await _siteConnector.DownloadView(SupportServices.Users, _identity, $"{path}");
            if (path.ToLower().StartsWith("accounts".ToLower()))
                indexViewModel.AccountsView = await _siteConnector.DownloadView(SupportServices.Accounts, _identity, $"{path}");
            if (path.ToLower().StartsWith("payments".ToLower()))
                indexViewModel.AccountsView = await _siteConnector.DownloadView(SupportServices.Payments, _identity, $"{path}");
            return View("index", indexViewModel);
        }


        [Route("endcall/{identity?}")]
        public async Task<ActionResult> EndCall(string identity)
        {
            await _siteConnector.Challenge(_identity, $"api/challenge/clear/{identity ?? "anonymous"}");

            return View("EndCall", new EndCallViewModel());
        }

    }

}