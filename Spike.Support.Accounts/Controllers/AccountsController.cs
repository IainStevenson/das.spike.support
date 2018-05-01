using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Accounts.Models;
using Spike.Support.Shared;
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

        private readonly ISiteConnector _siteConnector;

        public AccountsController()
        {
            _siteConnector = new SiteConnector();
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!MvcApplication.NavItems.Any())
            {
                MvcApplication.NavItems = _siteConnector.GetMenuTemplates<Dictionary<string, NavItem>>(
                                SupportServices.Portal,
                                "api/navigation/account") ?? new Dictionary<string, NavItem>();

            }
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
                Account = _accountViewModels.Accounts.FirstOrDefault(x => x.AccountId == id),
            };

            SetMenu(id, "Account.Account");

            return View("_accountDetails", accountDetailsViewModel);
        }


        [Route("accounts/{id:int}/payments")]
        public async Task<ActionResult> AccountPayments(int id)
        {
            var entityType = "Account.Payments";
            var identityName = GetIdentityOfCaller();

            if (await _siteConnector.Challenge(
                $"api/challenge/required/{entityType}/{id}/{identityName}"))
            {
                return RedirectToAction("AccountsChallenge", new
                {
                    entityType,
                    identifier = id,
                    identity = identityName,
                    returnTo = $"{_siteConnector.Services[SupportServices.Portal]}views/accounts/{id}/payments"
                });
            }

            var paymentsView = await _siteConnector.DownloadView(SupportServices.Portal, $"resources/payments/accounts/{id}");

            var accountPaymentsViewModel = new AccountPaymentsViewModel
            {
                Account = _accountViewModels.Accounts.FirstOrDefault(x => x.AccountId == id),
                View = paymentsView,

            };

            SetMenu(id, entityType);

            return View("_accountPayments", accountPaymentsViewModel);
        }

        private void SetMenu(int id, string selectedItem)
        {
            ViewBag.Menu = NavItem.TransformNavItems(
                MvcApplication.NavItems,
                _siteConnector.Services[SupportServices.Portal],
                new Dictionary<string, string>() { { "accountId", $"{id}" } }
            );
            ViewBag.ActiveMenuKey = selectedItem;
        }

        private string GetIdentityOfCaller()
        {
            return  "Anonymous";//Request.RequestContext?.HttpContext.User?.Identity?.Name ??
        }


        [Route("accounts/{id:int}/users")]
        public async Task<ActionResult> AccountUsers(int id)
        {

            var usersView = await _siteConnector.DownloadView(SupportServices.Portal, $"resources/users/{id}/accounts/");

            var accountUsersViewModel = new AccountUsersViewModel
            {
                Account = _accountViewModels.Accounts.FirstOrDefault(x => x.AccountId == id),
                View = usersView
            };
            SetMenu(id, "Account.Users");

            return View("_accountUsers", accountUsersViewModel);
        }





        public ActionResult AccountsChallenge(string entityType, string identity, int identifier, string returnTo)
        {

            var model = new AccountsChallengeViewModel()
            {
                AccountId = identifier,
                Challenge = "Challenge",
                Response = null,
                ReturnTo = returnTo,
                EntityType = entityType,
                Identity = identity,
                ResponseUrl = $"{_siteConnector.Services[SupportServices.Accounts]}accounts/challenge/response"
            };
            SetMenu(identifier, "Account.Payments");

            return View("_accountsChallenge", model);
        }

        [Route("accounts/challenge/response")]
        [HttpPost]
        public async Task<ActionResult> AccountsChallengeResponse(FormCollection formProperties)
        {
            var response = (formProperties ?? new FormCollection())["Response"] ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(response))
            {
                var id = (formProperties ?? new FormCollection())["AccountId"] ?? string.Empty;
                var entityType = (formProperties ?? new FormCollection())["EntityType"] ?? string.Empty;
                var identity = (formProperties ?? new FormCollection())["Identity"] ?? string.Empty;
                if (int.TryParse(id, out var accountId))
                {
                    // TODO: Verify response is valid for id

                    await _siteConnector.Challenge(
                        $"api/challenge/passed/{entityType}/{accountId}/{identity}");

                    var returnTo = (formProperties ?? new FormCollection())["ReturnTo"] ?? string.Empty;
                    return Redirect(returnTo);
                }
            }
            return Redirect("accounts/denied");
        }
    }
}