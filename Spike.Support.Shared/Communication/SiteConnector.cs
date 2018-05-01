using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Spike.Support.Shared.Communication
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

        public SiteConnector()
        {
            //TODO: Configure Services
            //TODO: implement token based security
        }
        public async Task<MvcHtmlString> DownloadView(SupportServices serviceName, string uri)
        {
            var baseUrl = Services[serviceName];
           
            return await DownloadView(baseUrl, uri);
        }

        public T GetMenuTemplates<T>(SupportServices serviceName, string uri)
        {
            var baseUrl = Services[serviceName];
            var client = new HttpClient
            {
                BaseAddress = baseUrl
            };
            AddCustomHeaders(client);

            string content = null;
            try
            {
                var response =  client.GetAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    content = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return content != null ?  JsonConvert.DeserializeObject<T>(content) : default(T);
        }

        private Dictionary<string, object> _customHeaders = new Dictionary<string, object>() ;
        public void SetHeader(string header, object value)
        {
            if (_customHeaders.ContainsKey(header))
            {
                _customHeaders[header] =  value;
            }
            else
            {
                _customHeaders.Add(header, value);
            }
        }

        public void ClearHeader(string header)
        {
            if (_customHeaders.ContainsKey(header))
            {
                _customHeaders.Remove(header);
            }
        }

        private async Task<MvcHtmlString> DownloadView(Uri resourceAddress, string uri)
        {
            var client = new HttpClient
            {
                BaseAddress = resourceAddress
            };

            AddCustomHeaders(client);
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

        private void AddCustomHeaders(HttpClient client)
        {
            foreach (var customHeader in _customHeaders)
            {
                client.DefaultRequestHeaders.Add(customHeader.Key, $"{customHeader.Value}");
            }
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