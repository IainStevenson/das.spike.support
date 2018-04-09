using System;
using System.CodeDom;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Spike.Support.Shared
{
    public class SiteConnector : ISiteConnector
    {
        public async Task<string> DownloadView(string baseUrl, string uri)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl, UriKind.Absolute)
            };

            string content = null;
            try
            {
                content = await client.GetStringAsync(uri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return content;
        }

        public async Task<string> DownloadView(Uri resourceAddress, string uri)
        {
            var client = new HttpClient
            {
                BaseAddress = resourceAddress
            };

            string content = null;
            try
            {
                content = await client.GetStringAsync(uri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return content;
        }

        public async Task<T> DownloadResource<T>(Uri resourceAddress, string uri) where T: class
        {
            var client = new HttpClient
            {
                BaseAddress = resourceAddress
            };

            string content = null;
            try
            {
               var  response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (typeof(T) == typeof(string))
            {
                return content as T;
            }

            return JsonConvert.DeserializeObject<T>(content);
        }


    }
}