using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Spike.Support.Shared.Models;

namespace Spike.Support.Shared.Communication
{
    public class SiteConnector : ISiteConnector
    {
        private readonly Dictionary<string, object> _customHeaders = new Dictionary<string, object>();
        private readonly IIdentityHandler _identityHandler;
        private readonly string _cookieName = "IdentityContextCookie";
        private readonly string _cookieDomain = ".localhost";  // change to real domain for deployment
        private readonly string _defaultIdentity = "anonymous";

        public SiteConnector()
        {
            _identityHandler = new CookieIdentityHandler(_cookieName, _cookieDomain, _defaultIdentity);
        }


        public Dictionary<SupportServices, Uri> Services { get; } = new Dictionary<SupportServices, Uri>
        {
            {SupportServices.Portal, new Uri("https://localhost:44394/")},
            {SupportServices.Accounts, new Uri("https://localhost:44317/")},
            {SupportServices.Users, new Uri("https://localhost:44309/")},
            {SupportServices.Payments, new Uri("https://localhost:44345/")}
        };

        public async Task<MvcHtmlString> DownloadView(SupportServices serviceName, string identity, string uri)
        {
            var baseUrl = Services[serviceName];
            return await DownloadView(baseUrl, identity, uri);
        }

        public T GetMenuTemplates<T>(SupportServices serviceName, string identity, string uri)
        {
            var baseAddress = Services[serviceName];
            var handler = new HttpClientHandler() { CookieContainer = new CookieContainer() };
            _identityHandler.SetIdentity(handler, baseAddress, identity);
            var client = new HttpClient(handler) { BaseAddress = baseAddress };

            AddCustomHeaders(client);

            string content = null;
            try
            {
                var response = client.GetAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode) content = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return content != null ? JsonConvert.DeserializeObject<T>(content) : default(T);
        }

        public void SetCustomHeader(string header, object value)
        {
            if (_customHeaders.ContainsKey(header))
                _customHeaders[header] = value;
            else
                _customHeaders.Add(header, value);
        }

        public void ClearCustomHeaders(string header)
        {
            if (_customHeaders.ContainsKey(header)) _customHeaders.Remove(header);
        }

        public async Task<bool> Challenge(string identity, string uri)
        {
            var baseAddress = Services[SupportServices.Portal];
            var handler = new HttpClientHandler() { CookieContainer = new CookieContainer() };
            _identityHandler.SetIdentity(handler, baseAddress, identity);
            var client = new HttpClient(handler) { BaseAddress = baseAddress };

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(content);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return true;
        }

        private async Task<MvcHtmlString> DownloadView(Uri resourceAddress, string identity, string uri)
        {
            var baseAddress = resourceAddress;
            var handler = new HttpClientHandler() { CookieContainer = new CookieContainer() };
            _identityHandler.SetIdentity(handler, baseAddress, identity);
            var client = new HttpClient(handler) { BaseAddress = baseAddress };

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
                client.DefaultRequestHeaders.Add(customHeader.Key, $"{customHeader.Value}");
        }
    }
}