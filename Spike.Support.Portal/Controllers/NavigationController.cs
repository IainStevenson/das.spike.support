using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Spike.Support.Shared.Communication;
using Spike.Support.Shared.Models;

namespace Spike.Support.Portal.Controllers
{
    [RoutePrefix("api/navigation")]
    public class NavigationController : ApiController
    {
        private readonly List<NavItem> _systemMenu = new List<NavItem>
        {
            new NavItem
            {
                Key = "User.User",
                Ordinal = 0,
                Text = "User",
                NavigateUrl = "views/users/{userId}",
                Roles = new string[]
                {
                    "Tier1", "Tier2"
                }
            },
            new NavItem
            {
                Key = "User.Accounts",
                Ordinal = 1,
                Text = "Account",
                NavigateUrl = "views/users/{userId}/accounts",
                Roles = new string[]
                {
                    "Tier1", "Tier2"
                }
            },
            new NavItem
            {
                Key = "Account.Account",
                Ordinal = 0,
                Text = "Account",
                NavigateUrl = "views/accounts/{accountId}",
                Roles = new string[]
                {
                    "Tier1", "Tier2"
                }
            },
            new NavItem
            {
                Key = "Account.Payments",
                Ordinal = 1,
                Text = "Payments",
                NavigateUrl = "views/accounts/{accountId}/payments",
                Roles = new string[]
                {
                     "Tier2"
                },

                NavItems = new List<NavItem>
                {
                    new NavItem
                    {
                        Key = "Account.Payments.All",
                        Ordinal = 0,
                        Text = "All",
                        NavigateUrl = "views/accounts/{accountId}/payments",
                        Roles = new string[]
                        {
                            "Tier2"
                        }
                    },
                    new NavItem
                    {
                        Key = "Account.Payments.In",
                        Ordinal = 1,
                        Text = "Recieved",
                        NavigateUrl = "views/accounts/{accountId}/payments/in",
                        Roles = new string[]
                        {
                            "Tier2"
                        }
                    },
                    new NavItem
                    {
                        Key = "Account.Payments.Out",
                        Ordinal = 2,
                        Text = "Made",
                        NavigateUrl = "views/accounts/{accountId}/payments/out",
                        Roles = new string[]
                        {
                            "Tier2"
                        }
                    }
                }
            },
            new NavItem
            {
                Key = "Account.Users",
                Ordinal = 2,
                Text = "Users",
                NavigateUrl = "views/accounts/{accountId}/users",
                Roles = new string[]
                {
                    "Tier1", "Tier2"
                }
            }
        };
        private readonly IIdentityHandler _identityHandler;
        private readonly string _cookieName = "IdentityContextCookie";
        private readonly string _cookieDomain = ".localhost";  // change to real domain for deployment // change to real domain for deployment
        private readonly string _defaultIdentity = "anonymous";
        private string _identity;
        private string[] _roles;

        public NavigationController()
        {
            _identityHandler = new CookieIdentityHandler(_cookieName, _cookieDomain, _defaultIdentity);
        }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            var identity = _identityHandler.GetIdentity(new HttpRequestWrapper(HttpContext.Current.Request));
            identity = HttpContext.Current.Server.UrlDecode(identity);
            Debug.WriteLine($"App-Debug: {(nameof(ChallengeController))} (API) {nameof(ExecuteAsync)} Recieves Identity {_identity}");

            _identity = identity.Split('|').First();
            _roles = identity.Split('|').Skip(1).First().Split(',');

            return base.ExecuteAsync(controllerContext, cancellationToken);
        }
        [Route("templates/{id?}")]
        [HttpGet]
        public async Task<Dictionary<string, NavItem>> Templates(string id = null)
        {
            Debug.WriteLine($"App-Debug: {(nameof(NavigationController))} {nameof(Templates)} {id}");
            // Key = "This.That.The.Other"
            // id = null
            // id = "This"
            // id = "This."
            // 
            var menuItems = _systemMenu
                .Where(x => x.Key.StartsWith($"{id ?? x.Key}",
                    StringComparison.CurrentCultureIgnoreCase) &&
                                x.Roles.Intersect(_roles).Any())
                .OrderBy(y => y.Ordinal)
                .ToDictionary(navItem => navItem.Key);
            return await Task.FromResult(menuItems);
        }
    }
}