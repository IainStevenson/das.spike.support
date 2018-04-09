using System;
using System.Threading.Tasks;

namespace Spike.Support.Shared
{
    public interface ISiteConnector
    {
        Task<string> DownloadView(string baseUrl, string uri);
        Task<string> DownloadView(Uri resourceAddress, string uri);
        Task<T> DownloadResource<T>(Uri resourceAddress, string uri) where T: class; 
    }
}