using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly int _maxChallengeTries = 3;

        public AccountsController()
        {
            _siteConnector = new SiteConnector();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!MvcApplication.NavItems.Any())
                MvcApplication.NavItems = _siteConnector.GetMenuTemplates<Dictionary<string, NavItem>>(
                                              SupportServices.Portal,
                                              "api/navigation/templates") ?? new Dictionary<string, NavItem>();
            base.OnActionExecuting(filterContext);
        }

        [Route("")]
        [Route("accounts")]
        public ActionResult AccountList()
        {
            return View("accounts", _accountViewModels);
        }


        [Route("accounts/{id:int}")]
        public ActionResult AccountDetail(int id)
        {
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
            var entityType = "Account.Payments";
            var identityName = GetIdentityOfCaller();
            if (await _siteConnector.Challenge(
                $"api/challenge/required/{entityType}/{id}/{identityName}"))
            {
                var model = new ChallengeViewModel
                {
                    AccountId = id,
                    Challenge = GetChallengeForEntityType(entityType),
                    ReturnTo = $"{_siteConnector.Services[SupportServices.Portal]}views/accounts/{id}/payments/in",
                    EntityType = entityType,
                    Identity = identityName,
                    Tries = tries,
                    MaxTries = _maxChallengeTries
                };

                return RedirectToAction("AccountsChallenge", model);
            }

            await _siteConnector.Challenge($"api/challenge/refresh/{entityType}/{id}/{identityName}");

            var paymentsView =
                await _siteConnector.DownloadView(SupportServices.Portal, $"resources/payments/accounts/{id}/in");

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
            var entityType = "Account.Payments";
            var identityName = GetIdentityOfCaller();

            if (await _siteConnector.Challenge(
                $"api/challenge/required/{entityType}/{id}/{identityName}"))
            {
                var model = new ChallengeViewModel
                {
                    AccountId = id,
                    Challenge = GetChallengeForEntityType(entityType),
                    ReturnTo = $"{_siteConnector.Services[SupportServices.Portal]}views/accounts/{id}/payments/out",
                    EntityType = entityType,
                    Identity = identityName,
                    Tries = tries,
                    MaxTries = _maxChallengeTries
                };
                return RedirectToAction("AccountsChallenge", model);
            }

            await _siteConnector.Challenge($"api/challenge/refresh/{entityType}/{id}/{identityName}");

            var paymentsView =
                await _siteConnector.DownloadView(SupportServices.Portal, $"resources/payments/accounts/{id}/out");

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
            var entityType = "Account.Payments";
            var identityName = GetIdentityOfCaller();

            if (await _siteConnector.Challenge(
                $"api/challenge/required/{entityType}/{id}/{identityName}"))
            {
                var model = new ChallengeViewModel
                {
                    AccountId = id,
                    Challenge = GetChallengeForEntityType(entityType),
                    ReturnTo = $"{_siteConnector.Services[SupportServices.Portal]}views/accounts/{id}/payments",
                    EntityType = entityType,
                    Identity = identityName,
                    Tries = tries,
                    MaxTries = _maxChallengeTries
                };
                return RedirectToAction("AccountsChallenge", model);
            }

            await _siteConnector.Challenge($"api/challenge/refresh/{entityType}/{id}/{identityName}");

            var paymentsView =
                await _siteConnector.DownloadView(SupportServices.Portal, $"resources/payments/accounts/{id}");

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

        private string GetIdentityOfCaller()
        {
            return "Anonymous"; //Request.RequestContext?.HttpContext.User?.Identity?.Name ??
        }

        [Route("accounts/{id:int}/users")]
        public async Task<ActionResult> AccountUsers(int id)
        {
            var usersView =
                await _siteConnector.DownloadView(
                    SupportServices.Portal,
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

        [Route("accounts/challenge/{entityType}/{identitifier}/{identity}/{returnTo}/{tries?}/{message?}")]
        public ActionResult AccountsChallenge(string entityType, string identity, int identifier, string returnTo, int tries, string message)
        {
            var formPostbackUrl = new Uri(_siteConnector.Services[SupportServices.Accounts],
                "accounts/challenge/response").AbsoluteUri;

            ChallengeViewModel model = new ChallengeViewModel
            {
                EntityType = entityType,
                AccountId = identifier,
                Identity = identity,
                ReturnTo = returnTo,
                Tries = tries,
                MaxTries = _maxChallengeTries,
                ResponseUrl = formPostbackUrl,
                Message = message

            };

            if (NavItem.IsAResourceRequest(Request))
                return View("_accountsChallenge", model);

            var identifiers = new Dictionary<string, string> { { "accountId", $"{model.AccountId}" } };

            var navItems = MvcApplication.NavItems
                .Where(x => x.Key.StartsWith($"{_menuType}"))
                .ToDictionary(x => x.Key, x => x.Value);

            var menuNavItems = NavItem.TransformNavItems(
                    navItems,
                    _siteConnector.Services[SupportServices.Portal],
                    identifiers
                ).Select(s => s.Value)
                .ToList();


            ViewBag.Menu = Menu.ConfigureMenu(
                menuNavItems,
                model.EntityType,
                new List<string> { model.EntityType },
                MenuOrientations.Vertical);

            return View("_accountsChallenge", model);
        }

        private string GetChallengeForEntityType(string entityType)
        {
            return $"Challenge for {entityType} ";
        }

        /// <summary>
        /// Processes the response, if its nothing or wrong the tries is incremented and passed back
        /// If Tries > MaxTries then the challenge is abanondeoned and passed back to the portals Challenge failed route.
        /// </summary>
        /// <param name="formProperties"></param>
        /// <returns></returns>
        [Route("accounts/challenge/response")]
        [HttpPost]
        public async Task<ActionResult> AccountsChallengeResponse(FormCollection formProperties)
        {
            if (formProperties == null) throw new ArgumentNullException(nameof(formProperties));

            var response = FormResponse(formProperties);

            var tries = int.Parse(formProperties["Tries"] ?? "1");

            var model = new ChallengeViewModel()
            {
                EntityType = formProperties["EntityType"] ?? string.Empty,
                Identity = formProperties["Identity"] ?? string.Empty,
                AccountId = int.Parse(formProperties["AccountId"] ?? "-1"),
                ReturnTo = formProperties["ReturnTo"],
                Tries = ++tries,
                MaxTries = _maxChallengeTries,
                Message = $"You did not provide a response"
            };

            if (string.IsNullOrWhiteSpace(response) || ResponseIsNotAcceptable(response))
            {
                
                if (model.Tries > model.MaxTries)
                {
                   
                    return Redirect(new Uri(_siteConnector.Services[SupportServices.Portal], 
                            $"views/challenge/failed").AbsoluteUri);
                }
                model.Message = $"Please try again";
                return ReturnToChallenge(model);
            }

            await _siteConnector.Challenge(
                    $"api/challenge/passed/{model.EntityType}/{model.AccountId}/{model.Identity}");

            return Redirect(model.ReturnTo);
        }

        private ActionResult ReturnToChallenge(ChallengeViewModel model)
        {
            return Redirect(new Uri(
                    _siteConnector.Services[SupportServices.Portal],
                    $"views/accounts/challenge/{model.EntityType}/{model.AccountId}/{model.Identity}/{model.ReturnTo}/{model.Tries}/{model.Message}")
                .AbsoluteUri
            );
        }

        private bool ResponseIsNotAcceptable(string response)
        {
            return response.StartsWith("f", StringComparison.InvariantCultureIgnoreCase);
        }

        private static string FormResponse(FormCollection formProperties)
        {
            return (formProperties ?? new FormCollection())["Response"] ?? string.Empty;
        }
    }

}