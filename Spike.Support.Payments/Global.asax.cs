using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Spike.Support.Shared;

namespace Spike.Support.Payments
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            HostingEnvironment
                .RegisterVirtualPathProvider(
                    new EmbeddedResourceViewPathProvider());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}