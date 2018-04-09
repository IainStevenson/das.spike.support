using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Shared;

namespace Spike.Support.Portal.Controllers
{
    public class ResourcesController : Controller
    {

        private readonly Dictionary<string, Uri> _addresses = new Dictionary<string, Uri>()
        {
            {"accounts", new Uri("https://localhost:44317/")},
            {"users", new Uri("https://localhost:44309/")},
            {"payments", new Uri("https://localhost:44345/")},
        };

        private readonly ISiteConnector _siteConnector;

        public ResourcesController()
        {
            _siteConnector = new SiteConnector();
        }

        [Route("resources/resource/{*path}")]
        public async Task<MvcHtmlString> Get(string path)
        {
            var source = path.Split('/').FirstOrDefault();

            if (string.IsNullOrWhiteSpace(source)) return new MvcHtmlString (string.Empty);


            if (_addresses.Keys.FirstOrDefault(x => x == source) == null)
                return await Task.Run(() => new MvcHtmlString(string.Empty));

            var resourceAddress = _addresses[source];

            MvcHtmlString resource = await _siteConnector.DownloadView(resourceAddress, $"{path}");

            return resource;
            
        }
    }
}
