using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Spike.Support.Portal.Controllers
{
    public class NavigationController : ApiController
    {
        [System.Web.Mvc.Route("api/navigation/{id}")]
        [HttpGet]
        public async Task<Dictionary<string, string>> NavigationItems(string id)
        {
            var menuItems = new Dictionary<string, string>() { };

            if (id == "user")
            {
                menuItems.Add("User.User", "/users/{userId}");
                menuItems.Add("User.Account", "/accounts/{userId}");
            }
            else
            {
                menuItems.Add("Account.Account", "/accounts/{accountId}");
                menuItems.Add("Account.Payments", "/payments/accounts/{accountId}");
                menuItems.Add("Account.Users", "/accounts/{accountId}/users");
            }
            
            return await Task.FromResult(menuItems);
        }
    }
}
