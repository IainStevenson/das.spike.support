using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Spike.Support.Shared
{
    public class SiteConnector : ISiteConnector
    {
        public Dictionary<SupportServices, Uri> Services { get; } = new Dictionary<SupportServices, Uri>
        {
            {SupportServices.Portal, new Uri("https://localhost:44394/")},
            {SupportServices.Accounts, new Uri("https://localhost:44317/")},
            {SupportServices.Users, new Uri("https://localhost:44309/")},
            {SupportServices.Payments, new Uri("https://localhost:44345/")}
        };

        public async Task<MvcHtmlString> DownloadView(SupportServices serviceName, string uri)
        {
            var baseUrl = Services[serviceName];
            var client = new HttpClient
            {
                BaseAddress = baseUrl
            };

            return await DownloadView(baseUrl, uri);
        }

        private async Task<MvcHtmlString> DownloadView(Uri resourceAddress, string uri)
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

            return new MvcHtmlString(content);
        }

        //public async Task<T> DownloadResource<T>(Uri resourceAddress, string uri) where T: class
        //{
        //    var client = new HttpClient
        //    {
        //        BaseAddress = resourceAddress
        //    };

        //    string content = null;
        //    try
        //    {
        //       var  response = await client.GetAsync(uri);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            content = await response.Content.ReadAsStringAsync();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }

        //    if (typeof(T) == typeof(string))
        //    {
        //        return content as T;
        //    }

        //    return JsonConvert.DeserializeObject<T>(content);
        //}
    }
}