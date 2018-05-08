using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Shared;
using Spike.Support.Shared.Communication;
using Spike.Support.Shared.Models;

namespace Spike.Support.Portal.Controllers
{
    public class ResourcesController : BaseController
    {
        private readonly ISiteConnector _siteConnector;

        public ResourcesController()
        {
            _siteConnector = new SiteConnector();
        }

        [Route("resources/{*path}")]
        public async Task<MvcHtmlString> Get(string path)
        {
            var source = path.Split('/').FirstOrDefault();

            if (string.IsNullOrWhiteSpace(source)) return new MvcHtmlString(string.Empty);

            var ok = Enum.TryParse(source, true, out SupportServices service);

            if (!ok) return await Task.Run(() => new MvcHtmlString(string.Empty));

            _siteConnector.SetHeader("X-Resource", null);

            var resource = await _siteConnector.DownloadView(service, $"{path}");

            _siteConnector.ClearHeader("X-Resource");

            return resource;
        }
    }
}