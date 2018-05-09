using System;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Spike.Support.Shared.Communication
{
    public class CookieIdentityHandler : IIdentityHandler
    {
        private readonly string _cookieName = "IdentityContextCookie";
        private readonly string _cookieDomain = ".localhost"; // change to real domain for deployment
        private readonly string _defaultIdentity = "anonymous";

        public CookieIdentityHandler(string cookieName, string cookieDomain, string defaultIdentity)
        {
            _cookieName = cookieName?? _cookieName;
            _cookieDomain = cookieDomain?? _cookieDomain;
            _defaultIdentity = defaultIdentity?? _defaultIdentity;
        }

        public string GetIdentity(HttpRequestBase request)
        {
            return HttpContext.Current.Server.UrlDecode(
                        (request.Cookies[_cookieName] ?? new HttpCookie(_cookieName, null)).Value?? _defaultIdentity);
        }

        public void SetIdentity(HttpClientHandler handler, Uri baseAddress, string cookieValue)
        {
            if (handler.CookieContainer == null)
            {
                handler.CookieContainer = new CookieContainer();
            }

            var value = HttpContext.Current.Server.UrlEncode(cookieValue);
            handler.CookieContainer.Add(baseAddress, new Cookie(_cookieName, value)
            {
                Domain = _cookieDomain, HttpOnly = true, Secure = true, 
            });
        }

    }
}