using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Spike.Support.Portal.Models;

namespace Spike.Support.Portal.Controllers
{
    public class ResourcesController : ApiController
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

        [System.Web.Http.Route("resources/{source}/{resource}")]
        public async Task<MvcHtmlString> Get(string source, string resource)
        {
            if (_addresses.Keys.FirstOrDefault(x => x == source) == null)
                return await Task.Run(() => new MvcHtmlString(string.Empty));

            var resourceAddress = _addresses[source];
            return await  _siteConnector.DownloadView(resourceAddress, $"{source}/{resource}");
            
        }
    }
}
