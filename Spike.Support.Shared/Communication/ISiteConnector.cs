using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Shared.Models;

namespace Spike.Support.Shared.Communication
{
    public interface ISiteConnector
    {
        Dictionary<SupportServices, Uri> Services { get; }
        Task<MvcHtmlString> DownloadView(SupportServices serviceName, string identity, string uri);
        T GetMenuTemplates<T>(SupportServices service, string identity, string uri);
        void SetCustomHeader(string header, object value);
        void ClearCustomHeaders(string header);
        Task<bool> Challenge(string identity, string uri);
    }
}