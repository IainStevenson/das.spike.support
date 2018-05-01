using System.IO;
using System.Reflection;
using System.Web.Hosting;

namespace Spike.Support.Shared
{
    public class EmbeddedResourceFile : VirtualFile
    {
        public EmbeddedResourceFile(string virtualPath) : base(virtualPath)
        {
        }

        public static string GetResourceName(string virtualPath)
        {
            if (!virtualPath.Contains("/Views/")) return null;

            var resourcename =
                virtualPath
                    .Substring(virtualPath.IndexOf("Views/"))
                    // NB: Your assembly name here
                    .Replace("Views/", "Spike.Support.Shared.Views.")
                    .Replace("/", ".");

            return resourcename;
        }

        public override Stream Open()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcename = GetResourceName(VirtualPath);
            return assembly.GetManifestResourceStream(resourcename);
        }
    }
}