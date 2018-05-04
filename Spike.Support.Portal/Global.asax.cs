using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Spike.Support.Portal.Models;

namespace Spike.Support.Portal
{
    public class MvcApplication : HttpApplication
    {
        public static readonly Dictionary<Guid, SupportAgentChallenge> SupportAgentChallenges =
            new Dictionary<Guid, SupportAgentChallenge>();

       

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);


            GlobalConfiguration.Configuration.MapHttpAttributeRoutes();

            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );

            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                "ChallengeApi",
                "api/challenge"
            );

            RouteConfig.RegisterRoutes(RouteTable.Routes);


            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}