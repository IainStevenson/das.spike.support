using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Spike.Support.Shared
{
    public interface ISiteConnector
    {
        Dictionary<SupportServices, Uri> Services { get; }
        Task<MvcHtmlString> DownloadView(SupportServices serviceName, string uri);
    }
}