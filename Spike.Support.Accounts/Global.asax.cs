﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Spike.Support.Shared;

namespace Spike.Support.Accounts
{
    public class MvcApplication : HttpApplication
    {
        
        public static readonly ConcurrentDictionary<Guid, AgentAccountChallenge> AccountChallenges  = new ConcurrentDictionary<Guid, AgentAccountChallenge>();

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