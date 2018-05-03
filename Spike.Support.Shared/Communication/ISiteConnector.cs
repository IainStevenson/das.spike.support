using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Spike.Support.Shared.Communication
{
    public interface ISiteConnector
    {
        Dictionary<SupportServices, Uri> Services { get; }
        Task<MvcHtmlString> DownloadView(SupportServices serviceName, string uri);
        T GetMenuTemplates<T>(SupportServices service, string uri);
        void SetHeader(string header, object value);
        void ClearHeader(string header);
        Task<bool> Challenge(string uri);
    }
}