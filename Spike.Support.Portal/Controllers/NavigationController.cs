using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
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
                NavigateUrl = "views/users/{userId}"
            },
            new NavItem
            {
                Key = "User.Accounts",
                Ordinal = 1,
                Text = "Account",
                NavigateUrl = "views/users/{userId}/accounts"
            },
            new NavItem
            {
                Key = "Account.Account",
                Ordinal = 0,
                Text = "Account",
                NavigateUrl = "views/accounts/{accountId}"
            },
            new NavItem
            {
                Key = "Account.Payments",
                Ordinal = 1,
                Text = "Payments",
                NavigateUrl = "views/accounts/{accountId}/payments",
                NavItems = new List<NavItem>
                {
                    new NavItem
                    {
                        Key = "Account.Payments.All",
                        Ordinal = 0,
                        Text = "All",
                        NavigateUrl = "views/accounts/{accountId}/payments"
                    },
                    new NavItem
                    {
                        Key = "Account.Payments.In",
                        Ordinal = 1,
                        Text = "Recieved",
                        NavigateUrl = "views/accounts/{accountId}/payments/in"
                    },
                    new NavItem
                    {
                        Key = "Account.Payments.Out",
                        Ordinal = 2,
                        Text = "Made",
                        NavigateUrl = "views/accounts/{accountId}/payments/out"
                    }
                }
            },
            new NavItem
            {
                Key = "Account.Users",
                Ordinal = 2,
                Text = "Users",
                NavigateUrl = "views/accounts/{accountId}/users"
            }
        };

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
                    StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(y => y.Ordinal)
                .ToDictionary(navItem => navItem.Key);
            return await Task.FromResult(menuItems);
        }
    }
}