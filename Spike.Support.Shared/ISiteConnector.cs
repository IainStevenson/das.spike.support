using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Spike.Support.Shared
{

    public enum SupportServices
    {
        Portal,
        Accounts,
        Payments,
        Users

    }

    public interface ISiteConnector
    {
        Dictionary<SupportServices, Uri> Services { get; }
        Task<MvcHtmlString> DownloadView(SupportServices serviceName, string uri);
    }
}