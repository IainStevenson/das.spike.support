using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Spike.Support.Shared;

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

        [System.Web.Http.Route("resources/resource/{*path}")]
        public async Task<string> Get(string path)
        {
            var source = path.Split('/').FirstOrDefault();

            if (string.IsNullOrWhiteSpace(source)) return string.Empty;


            if (_addresses.Keys.FirstOrDefault(x => x == source) == null)
                return await Task.Run(() => string.Empty);

            var resourceAddress = _addresses[source];

            var resource = await  _siteConnector.DownloadResource<string>(resourceAddress, $"{path}");

            return resource;
            
        }
    }
}
