using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Spike.Support.Shared.Models;

namespace Spike.Support.Portal.Controllers
{
    public class NavigationController : ApiController
    {

        private List<NavItem> _systemMenu = new List<NavItem>()
        {
            new NavItem(){ Key = "User.User", Ordinal = 0, Text = "User", NavigateUrl = "views/users/{userId}"},
            new NavItem(){ Key = "User.Accounts", Ordinal = 1, Text = "Account", NavigateUrl = "views/users/{userId}/accounts"},
            new NavItem(){ Key = "Account.Account", Ordinal = 0, Text = "Account", NavigateUrl = "views/accounts/{accountId}"},
            new NavItem(){ Key = "Account.Payments", Ordinal = 1, Text = "Payments", NavigateUrl = "views/accounts/{accountId}/payments"},
            new NavItem(){ Key = "Account.Users", Ordinal = 2, Text = "Users", NavigateUrl = "views/accounts/{accountId}/users"},
        };

        [Route("api/navigation/{id}")]
        [HttpGet]
        public async Task<Dictionary<string, NavItem>> NavigationItems(string id)
        {
            var menuItems = new Dictionary<string, NavItem>() { };

            foreach (var navItem in _systemMenu
                .Where(
                        x=>x.Key.StartsWith($"{id??x.Key}.".Replace("..","."), StringComparison.CurrentCultureIgnoreCase)
                    ).OrderBy(y=>y.Ordinal))
            {
                menuItems.Add(navItem.Key, navItem);
            }

            return await Task.FromResult(menuItems);
        }
    }
}
