using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Spike.Support.Portal.Models
{
    public class SiteConnector : ISiteConnector
    {
        public async Task<MvcHtmlString> DownloadView(string baseUrl, string uri)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl, UriKind.Absolute)
            };

            var content = await client.GetStringAsync(uri);

            return new MvcHtmlString(content);
        }

        public async Task<MvcHtmlString> DownloadView(Uri resourceAddress, string uri)
        {
            var client = new HttpClient
            {
                BaseAddress = resourceAddress
            };

            var content = await client.GetStringAsync(uri);

            return new MvcHtmlString(content);
        }
    }
}