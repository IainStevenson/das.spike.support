using System.Web.Mvc;
using System.Web.Routing;

namespace Spike.Support.Portal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                "Views",
                "{controller}/{*path}",
                new {controller = "Views", action = "Index", source = string.Empty}
            );
            routes.MapRoute(
                "ResourcesApi",
                "resources/{*path}",
                new {controller = "Resources", action = "Index", source = string.Empty});
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Views", action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}