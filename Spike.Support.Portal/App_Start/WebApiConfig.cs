using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Spike.Support.Portal
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute(
                name: "ResourcesApi",
                routeTemplate: "api/resources/resource/{*path}",
                defaults: new { controller = "Resources", action = "Index", source = string.Empty });


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
