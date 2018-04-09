using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Spike.Support.Portal.Models
{
    public interface ISiteConnector
    {
        Task<MvcHtmlString> DownloadView(string baseUrl, string uri);
        Task<MvcHtmlString> DownloadView(Uri resourceAddress, string vuri);
    }
}