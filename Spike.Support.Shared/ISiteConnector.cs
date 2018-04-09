using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Spike.Support.Shared
{
    public interface ISiteConnector
    {
        Task<MvcHtmlString> DownloadView(string baseUrl, string uri);
        Task<MvcHtmlString> DownloadView(Uri resourceAddress, string uri);
       // Task<T> DownloadResource<T>(Uri resourceAddress, string uri) where T: class; 
    }
}