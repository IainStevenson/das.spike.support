using System;
using System.Net.Http;
using System.Web;

namespace Spike.Support.Shared.Communication
{
    public interface IIdentityHandler
    {
        string GetIdentity(HttpRequestBase request);

        void SetIdentity(HttpClientHandler handler, Uri baseAddress, string cookieValue);
        
    }
}