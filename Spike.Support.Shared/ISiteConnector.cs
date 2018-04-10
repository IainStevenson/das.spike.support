using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Spike.Support.Shared
{
    public interface ISiteConnector
    {
        Task<MvcHtmlString> DownloadView(string baseUrl, string uri);

        Task<MvcHtmlString> DownloadView(Uri resourceAddress, string uri);
        string Reason { get; set; }
        Task Submit(Uri resourceAddress, string uri , string body);
    }
}