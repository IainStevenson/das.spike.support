using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Shared;
using Spike.Support.Shared.Models;
using Spike.Support.Users.Models;

namespace Spike.Support.Users.Controllers
{
    public class UsersController : ViewControllerBase
    {
        private readonly UsersViewModel _usersViewModel = new UsersViewModel
        {
            Users = Enumerable.Range(1, 10).Select(x => new UserViewModel
            {
                UserId = x,
                AccountId = x,
                UserName = $"User {x}",
                Created = DateTime.Now.AddMonths(-x)
            }).ToList()
        };

        [Route("")]
        [Route("users")]
        public ActionResult Users()
        {
            return View("users", _usersViewModel);
        }

        [Route("users/{id:int}")]
        public ActionResult User(int id)
        {
            ViewBag.Menu = NavItem.GetItems(_menuItems, new Dictionary<string, string>() { { "userId", $"{id}" } });
            ViewBag.ActiveMenu = "user";

            return View("users", new UsersViewModel
            {
                Users = new List<UserViewModel>
                {
                    _usersViewModel.Users.FirstOrDefault(x => x.UserId == id)
                }
            });
        }


        [Route("users/{id:int}/accounts")]
        public ActionResult UserAccounts(int id)
        {
            ViewBag.Menu = NavItem.GetItems(_menuItems, new Dictionary<string, string>() { { "userId", $"{id}" } });
            ViewBag.ActiveMenu = "accounts";

            return View("users", new UsersViewModel
            {
                Users = _usersViewModel.Users.Where(x => x.AccountId == id).ToList()
            });
        }

        [Route("endcall/{identity?}")]
        public async Task<ActionResult> EndCall(string identity)
        {
            return View("EndCall");
        }

    }
}