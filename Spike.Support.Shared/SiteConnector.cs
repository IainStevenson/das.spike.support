using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Spike.Support.Shared
{
    public class SiteConnector : ISiteConnector
    {
        public async Task<MvcHtmlString> DownloadView(string baseUrl, string uri)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl, UriKind.Absolute)
            };

            string content = null;
            try
            {
                var response = await client.GetAsync(uri);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Reason = response.ReasonPhrase;
                }
                else if (response.IsSuccessStatusCode)
                {
                    Reason = null;
                    content = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Reason = "Non OK Status";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new MvcHtmlString(content);
        }

        public async Task<MvcHtmlString> DownloadView(Uri resourceAddress, string uri)
        {
            var client = new HttpClient
            {
                BaseAddress = resourceAddress
            };
            Reason = null;
            string content = null;
            try
            {
                var response= await client.GetAsync(uri);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Reason = response.ReasonPhrase;
                }
                else if (response.IsSuccessStatusCode)
                {
                    Reason = null;
                    content = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Reason = "Non OK Status";
                }

            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"{e.Message}");
                Reason = e.Message;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new MvcHtmlString(content);
        }

        public string Reason { get; set; }
        public Task Submit(Uri resourceAddress, string uri, string body)
        {
            throw new NotImplementedException();
        }
    }
}