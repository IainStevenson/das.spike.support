using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Hosting;

namespace Spike.Support.Shared
{
    public class EmbeddedResourceViewPathProvider : VirtualPathProvider
    {
        private readonly Lazy<string[]> _resourceNames = new Lazy<string[]>(() => Assembly.GetExecutingAssembly().GetManifestResourceNames(), LazyThreadSafetyMode.ExecutionAndPublication);

        private bool ResourceFileExists(string virtualPath)
        {
            var resourcename = EmbeddedResourceFile.GetResourceName(virtualPath);
            var result = resourcename != null && _resourceNames.Value.Contains(resourcename);
            return result;
        }

        public override bool FileExists(string virtualPath)
        {
            return base.FileExists(virtualPath) || ResourceFileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return !base.FileExists(virtualPath) ?
                new EmbeddedResourceFile(virtualPath) :
                base.GetFile(virtualPath);
        }
    }
}
