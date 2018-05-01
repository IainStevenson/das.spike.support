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
            new NavItem(){ Key = "User.User", Text = "User", NavigateUrl = "views/users/{userId}"},
            new NavItem(){ Key = "User.Account", Text = "Account", NavigateUrl = "views/accounts/{userId}"},
            new NavItem(){ Key = "Account.Account", Text = "Account", NavigateUrl = "views/accounts/{accountId}"},
            new NavItem(){ Key = "Account.Payments", Text = "Payments", NavigateUrl = "views/payments/accounts/{accountId}"},
            new NavItem(){ Key = "Account.Users", Text = "Users", NavigateUrl = "views/accounts/{accountId}/users"},
        };

        [System.Web.Mvc.Route("api/navigation/{id}")]
        [HttpGet]
        public async Task<Dictionary<string, NavItem>> NavigationItems(string id)
        {
            var menuItems = new Dictionary<string, NavItem>() { };

            foreach (var navItem in _systemMenu
                .Where(
                        x=>x.Key.StartsWith($"{id??x.Key}.".Replace("..","."), StringComparison.CurrentCultureIgnoreCase)
                    ))
            {
                menuItems.Add(navItem.Key, navItem);
            }

            return await Task.FromResult(menuItems);
        }
    }
}
