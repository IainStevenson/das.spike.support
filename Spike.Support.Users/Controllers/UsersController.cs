using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly ISiteConnector _siteConnector;

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
        private readonly string _cookieName = "IdentityContextCookie";
        private readonly string _cookieDomain = ".localhost";  // change to real domain for deployment
        private readonly string _defaultIdentity = "anonymous";
        private readonly IIdentityHandler _identityHandler;
        private string _identity;

        public UsersController()
        {
            _siteConnector = new SiteConnector();
            _identityHandler = new CookieIdentityHandler(_cookieName, _cookieDomain, _defaultIdentity);

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _identity = _identityHandler.GetIdentity(Request);
            Debug.WriteLine($"App-Debug: {(nameof(UsersController))} {nameof(OnActionExecuting)} Recieves Identity {_identity}");
            if (!MvcApplication.NavItems.Any())
                MvcApplication.NavItems = _siteConnector.GetMenuTemplates<Dictionary<string, NavItem>>(
                                              SupportServices.Portal, 
                                              _identity,
                                              "api/navigation/templates") ?? new Dictionary<string, NavItem>();
            base.OnActionExecuting(filterContext);
        }

        [Route("")]
        [Route("users")]
        public ActionResult Users()
        {
            Debug.WriteLine($"App-Debug: {(nameof(UsersController))} {nameof(Users)}");

            return View("users", _usersViewModel);
        }

        [Route("users/{id:int}")]
        public ActionResult User(int id)
        {
            Debug.WriteLine($"App-Debug: {(nameof(UsersController))} {nameof(User)} {id}");
            if (!NavItem.IsAResourceRequest(Request))
            {
                var menuSelector = "User";
                var identifiers = new Dictionary<string, string> {{"userId", $"{id}"}};
                var menuNavItems = ViewBag.Menu = NavItem.TransformNavItems(
                        MvcApplication.NavItems.Where(x => x.Key.StartsWith($"{menuSelector}"))
                            .ToDictionary(x => x.Key, x => x.Value),
                        _siteConnector.Services[SupportServices.Portal],
                        identifiers)
                    .Select(s => s.Value).ToList();

                ViewBag.Menu = Menu.ConfigureMenu(menuNavItems, "User.User",
                    new List<string> {"User.User"},
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
            Debug.WriteLine($"App-Debug: {(nameof(UsersController))} {nameof(UserAccounts)} {id}");
            if (!NavItem.IsAResourceRequest(Request))
            {
                var menuSelector = "User";
                var identifiers = new Dictionary<string, string> {{"userId", $"{id}"}};
                var menuNavItems = ViewBag.Menu = NavItem.TransformNavItems(
                        MvcApplication.NavItems.Where(x => x.Key.StartsWith($"{menuSelector}"))
                            .ToDictionary(x => x.Key, x => x.Value),
                        _siteConnector.Services[SupportServices.Portal],
                        identifiers)
                    .Select(s => s.Value).ToList();

                ViewBag.Menu = Menu.ConfigureMenu(menuNavItems, "User.Accounts",
                    new List<string> {"User.Accounts"},
                    MenuOrientations.Vertical);
            }

            return View("users", new UsersViewModel
            {
                Users = _usersViewModel.Users.Where(x => x.AccountId == id).ToList()
            });
        }
    }
}