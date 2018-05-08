﻿using System;
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
        private string _cookieName = "IdentityContextCookie";
        private string _cookeValue = "not-anonymous";

        public void SetCookie(string name, string value)
        {
            _cookieName = name;
            _cookeValue = value;
        }

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

            return await DownloadView(baseUrl, uri);
        }

        public T GetMenuTemplates<T>(SupportServices serviceName, string uri)
        {
            
            var baseAddress = Services[serviceName];
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler) { BaseAddress = baseAddress };
            cookieContainer.Add(baseAddress, new Cookie(_cookieName, _cookeValue));

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

        public void SetHeader(string header, object value)
        {
            if (_customHeaders.ContainsKey(header))
                _customHeaders[header] = value;
            else
                _customHeaders.Add(header, value);
        }

        public void ClearHeader(string header)
        {
            if (_customHeaders.ContainsKey(header)) _customHeaders.Remove(header);
        }

        public async Task<bool> Challenge(string uri)
        {
            var baseAddress = Services[SupportServices.Portal];
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler) { BaseAddress = baseAddress };
            cookieContainer.Add(baseAddress, new Cookie(_cookieName, _cookeValue));

            

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

        private async Task<MvcHtmlString> DownloadView(Uri resourceAddress, string uri)
        {
            var baseAddress = resourceAddress;
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler) { BaseAddress = baseAddress };
            cookieContainer.Add(baseAddress, new Cookie(_cookieName, _cookeValue));

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