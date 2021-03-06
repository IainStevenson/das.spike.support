﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Spike.Support.Accounts.Models;
using Spike.Support.Shared.Communication;
using Spike.Support.Shared.Models;

namespace Spike.Support.Accounts.Controllers
{
    public class AccountsController : Controller
    {
        private readonly AccountsViewModel _accountViewModels = new AccountsViewModel
        {
            Accounts = Enumerable.Range(1, 10).Select(x =>
                new AccountViewModel
                {
                    AccountId = x,
                    Created = DateTime.Now.AddMonths(-x),
                    AccountName = $"Account {x}"
                }).ToList()
        };

        private readonly string _menuType = "Account";

        private readonly ISiteConnector _siteConnector;
    
        public AccountsController()
        {
            _siteConnector = new SiteConnector();
        }

        private string _identity = "anonymous";
        private static string _cookieName = "IdentityContextCookie";

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _identity = (Request.Cookies[_cookieName]?? new HttpCookie(_cookieName)).Value?? _identity;

            Debug.WriteLine($"App-Debug: {(nameof(AccountsController))} {nameof(OnActionExecuting)} Recieves Identity {_identity}");

            if (!MvcApplication.NavItems.Any())
                MvcApplication.NavItems = _siteConnector.GetMenuTemplates<Dictionary<string, NavItem>>(
                                              SupportServices.Portal, _identity,
                                              "api/navigation/templates") ?? new Dictionary<string, NavItem>();

            base.OnActionExecuting(filterContext);
        }

        [Route("")]
        [Route("accounts")]
        public ActionResult AccountList()
        {
            Debug.WriteLine($"App-Debug: {(nameof(AccountsController))} {nameof(OnActionExecuting)} Recieves Identity {_identity}");
            return View("accounts", _accountViewModels);
        }


        [Route("accounts/{id:int}")]
        public ActionResult AccountDetail(int id)
        {
            Debug.WriteLine($"App-Debug: {(nameof(AccountsController))} {nameof(AccountDetail)} {id}");
            var accountDetailsViewModel = new AccountDetailViewModel
            {
                Account = _accountViewModels.Accounts.FirstOrDefault(x => x.AccountId == id)
            };

            if (NavItem.IsAResourceRequest(Request))
                return View("_accountDetails", accountDetailsViewModel);

            var entityType = "Account.Account";

            var identifiers = new Dictionary<string, string>
            {
                {"accountId", $"{id}"}
            };

            var menuNavItems = NavItem.TransformNavItems(
                MvcApplication.NavItems
                    .Where(x => x.Key.StartsWith($"{_menuType}"))
                    .ToDictionary(x => x.Key, x => x.Value),
                _siteConnector.Services[SupportServices.Portal],
                identifiers
            ).Select(s => s.Value).ToList();

            ViewBag.Menu = Menu.ConfigureMenu(menuNavItems, entityType, new List<string> { entityType },
                MenuOrientations.Vertical);


            return View("_accountDetails", accountDetailsViewModel);
        }

        [Route("accounts/{id:int}/payments/in")]
        public async Task<ActionResult> AccountPaymentsIn(int id, int tries = 1)
        {
            Debug.WriteLine($"App-Debug: {(nameof(AccountsController))} {nameof(AccountPaymentsIn)} {id} {tries}");
            var entityType = "Account.Payments";
            
            if (await _siteConnector.Challenge(_identity, $"api/challenge/required/{entityType}/{id}"))
            {
                var model = new ChallengeViewModel
                {
                    MenuType = _menuType,
                    EntityType = entityType,
                    Identifier = $"{id}",
                    Identity = _identity,
                    ReturnTo = $"{_siteConnector.Services[SupportServices.Portal]}views/accounts/{id}/payments/int",
                    Tries = tries,
                };
                MvcApplication.Challenges.Add(model.ChallengeId, model);
                return RedirectToAction("Challenge", "Challenge", new { model.ChallengeId });
            }

            await _siteConnector.Challenge(_identity, $"api/challenge/refresh/{entityType}/{id}");

            var paymentsView =
                await _siteConnector.DownloadView(SupportServices.Portal, _identity, $"resources/payments/accounts/{id}/in");

            var accountPaymentsViewModel = new AccountPaymentsViewModel
            {
                Account = _accountViewModels.Accounts.FirstOrDefault(x => x.AccountId == id),
                View = paymentsView
            };
            if (NavItem.IsAResourceRequest(Request)) return View("_accountPayments", accountPaymentsViewModel);

            var identifiers = new Dictionary<string, string> { { "accountId", $"{id}" } };
            var menuNavItems = NavItem.TransformNavItems(
                MvcApplication.NavItems
                    .Where(x => x.Key.StartsWith($"{_menuType}"))
                    .ToDictionary(x => x.Key, x => x.Value),
                _siteConnector.Services[SupportServices.Portal],
                identifiers
            ).Select(s => s.Value).ToList();

            ViewBag.Menu = Menu.ConfigureMenu(menuNavItems, entityType,
                new List<string> { $"{entityType}", $"{entityType}.In" }, MenuOrientations.Horizontal);

            return View("_accountPayments", accountPaymentsViewModel);
        }

        [Route("accounts/{id:int}/payments/out")]
        public async Task<ActionResult> AccountPaymentsOut(int id, int tries = 1)
        {
            Debug.WriteLine($"App-Debug: {(nameof(AccountsController))} {nameof(AccountPaymentsOut)} {id} {tries}");
            var entityType = "Account.Payments";
            
            if (await _siteConnector.Challenge(_identity, $"api/challenge/required/{entityType}/{id}"))
            {
                var model = new ChallengeViewModel
                {
                    MenuType = _menuType,
                    EntityType = entityType,
                    Identifier = $"{id}",
                    Identity = _identity,
                    ReturnTo = $"{_siteConnector.Services[SupportServices.Portal]}views/accounts/{id}/payments/out",
                    Tries = tries,
                };
                MvcApplication.Challenges.Add(model.ChallengeId, model);
                return RedirectToAction("Challenge", "Challenge", new { model.ChallengeId });
            }

            await _siteConnector.Challenge(_identity, $"api/challenge/refresh/{entityType}/{id}");

            var paymentsView =
                await _siteConnector.DownloadView(SupportServices.Portal, _identity, $"resources/payments/accounts/{id}/out");

            var accountPaymentsViewModel = new AccountPaymentsViewModel
            {
                Account = _accountViewModels.Accounts.FirstOrDefault(x => x.AccountId == id),
                View = paymentsView
            };
            if (NavItem.IsAResourceRequest(Request)) return View("_accountPayments", accountPaymentsViewModel);

            var identifiers = new Dictionary<string, string> { { "accountId", $"{id}" } };
            var menuNavItems = NavItem.TransformNavItems(
                MvcApplication.NavItems
                    .Where(x => x.Key.StartsWith($"{_menuType}"))
                    .ToDictionary(x => x.Key, x => x.Value),
                _siteConnector.Services[SupportServices.Portal],
                identifiers
            ).Select(s => s.Value).ToList();

            ViewBag.Menu = Menu.ConfigureMenu(menuNavItems, entityType,
                new List<string> { $"{entityType}", $"{entityType}.Out" },
                MenuOrientations.Horizontal);
            return View("_accountPayments", accountPaymentsViewModel);
        }

        [Route("accounts/{id:int}/payments")]
        public async Task<ActionResult> AccountPayments(int id, int tries = 1)
        {
            Debug.WriteLine($"App-Debug: {(nameof(AccountsController))} {nameof(AccountPayments)} {id} {tries}");
            var entityType = "Account.Payments";
            
            if (await _siteConnector.Challenge(
                _identity, $"api/challenge/required/{entityType}/{id}"))
            {
                var model = new ChallengeViewModel
                {
                    MenuType = _menuType,
                    EntityType = entityType,
                    Identifier = $"{id}",
                    Identity = _identity,
                    ReturnTo = $"{_siteConnector.Services[SupportServices.Portal]}views/accounts/{id}/payments",
                    Tries = tries
                };
                MvcApplication.Challenges.Add(model.ChallengeId, model);
                return RedirectToAction("Challenge", "Challenge", new { model.ChallengeId });
            }

            await _siteConnector.Challenge(_identity, $"api/challenge/refresh/{entityType}/{id}");

            var paymentsView =
                await _siteConnector.DownloadView(SupportServices.Portal, _identity, $"resources/payments/accounts/{id}");

            var accountPaymentsViewModel = new AccountPaymentsViewModel
            {
                Account = _accountViewModels.Accounts.FirstOrDefault(x => x.AccountId == id),
                View = paymentsView
            };

            if (NavItem.IsAResourceRequest(Request)) return View("_accountPayments", accountPaymentsViewModel);

            var identifiers = new Dictionary<string, string> { { "accountId", $"{id}" } };
            var menuNavItems = NavItem.TransformNavItems(
                MvcApplication.NavItems
                    .Where(x => x.Key.StartsWith($"{_menuType}"))
                    .ToDictionary(x => x.Key, x => x.Value),
                _siteConnector.Services[SupportServices.Portal],
                identifiers
            ).Select(s => s.Value).ToList();
            ViewBag.Menu = Menu.ConfigureMenu(menuNavItems, entityType,
                new List<string> { $"{entityType}", $"{entityType}.All" },
                MenuOrientations.Horizontal);

            return View("_accountPayments", accountPaymentsViewModel);
        }

       

        [Route("accounts/{id:int}/users")]
        public async Task<ActionResult> AccountUsers(int id)
        {
            Debug.WriteLine($"App-Debug: {(nameof(AccountsController))} {nameof(AccountUsers)} {id}");
            var usersView =
                await _siteConnector.DownloadView(
                    SupportServices.Portal, _identity,
                    $"resources/users/{id}/accounts/");

            var accountUsersViewModel = new AccountUsersViewModel
            {
                Account = _accountViewModels.Accounts.FirstOrDefault(x => x.AccountId == id),
                View = usersView
            };

            if (NavItem.IsAResourceRequest(Request)) return View("_accountUsers", accountUsersViewModel);

            var entityType = "Account.User";


            var identifiers = new Dictionary<string, string> { { "accountId", $"{id}" } };
            var menuNavItems = NavItem.TransformNavItems(
                MvcApplication.NavItems
                    .Where(x => x.Key.StartsWith($"{_menuType}"))
                    .ToDictionary(x => x.Key, x => x.Value),
                _siteConnector.Services[SupportServices.Portal],
                identifiers
            ).Select(s => s.Value).ToList();
            ViewBag.Menu = Menu.ConfigureMenu(menuNavItems,
                entityType,
                new List<string> { entityType },
                MenuOrientations.Vertical);

            return View("_accountUsers", accountUsersViewModel);
        }
    }
}