using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Spike.Support.Accounts
{
    public class AgentAccountChallenge
    {
        public int AccountId { get; set; }
        public string Identity { get; set; }
        public DateTimeOffset Until { get; set; }
    }

    public class MvcApplication : HttpApplication
    {
        
        public static readonly ConcurrentDictionary<Guid, AgentAccountChallenge> AccountChallenges  = new ConcurrentDictionary<Guid, AgentAccountChallenge>();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}