using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Spike.Support.Shared;
using Spike.Support.Shared.Communication;
using Spike.Support.Shared.Models;
using Spike.Support.Users.Models;

namespace Spike.Support.Users.Controllers
{
    public class UsersController : Controller
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

        private readonly ISiteConnector _siteConnector;

        public UsersController()
        {
            _siteConnector = new SiteConnector();
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!MvcApplication.NavItems.Any())
            {
                MvcApplication.NavItems = _siteConnector.GetMenuTemplates<Dictionary<string, NavItem>>(
                    SupportServices.Portal,
                    "api/navigation/templates") ?? new Dictionary<string, NavItem>();
            }
            base.OnActionExecuting(filterContext);
        }
        [Route("")]
        [Route("users")]
        public ActionResult Users()
        {
            return View("users", _usersViewModel);
        }

        [Route("users/{id:int}")]
        public ActionResult User(int id)
        {

            if (!Request.Headers.AllKeys.Contains("X-Resource"))
            {
                var menuSelector = "User";
                var identifiers = new Dictionary<string, string>() { { "userId", $"{id}" } };
                var menuNavItems = ViewBag.Menu = NavItem.TransformNavItems(
                        MvcApplication.NavItems.Where(x => x.Key.StartsWith($"{menuSelector}")).ToDictionary(x => x.Key, x => x.Value),
                        _siteConnector.Services[SupportServices.Portal],
                        identifiers)
                    .Select(s => s.Value).ToList();

                ViewBag.Menu = Menu.ConfigureMenu(menuNavItems, "User.User",
                    new List<string>() { "User.User" },
                    MenuOrientations.Vertical);
            }

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
            if (!Request.Headers.AllKeys.Contains("X-Resource"))
            {
                var menuSelector = "User";
                var identifiers = new Dictionary<string, string>() { { "userId", $"{id}" } };
                var menuNavItems = ViewBag.Menu = NavItem.TransformNavItems(
                        MvcApplication.NavItems.Where(x => x.Key.StartsWith($"{menuSelector}")).ToDictionary(x => x.Key, x => x.Value),
                        _siteConnector.Services[SupportServices.Portal],
                        identifiers)
                    .Select(s => s.Value).ToList();

                ViewBag.Menu = Menu.ConfigureMenu(menuNavItems, "User.Accounts",
                    new List<string>() { "User.Accounts" },
                    MenuOrientations.Vertical);
            }

            return View("users", new UsersViewModel
            {
                Users = _usersViewModel.Users.Where(x => x.AccountId == id).ToList()
            });
        }
    }
}