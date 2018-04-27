using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Accounts.Models;
using Spike.Support.Shared;
using Spike.Support.Shared.Communication;

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

        [Route("")]
        [Route("accounts")]
        public ActionResult Index()
        {
            return View("accounts", _accountViewModels);
        }

        [Route("accounts/{id:int}")]
        public async Task<ActionResult> Index(int id)
        {
            if (ChallengeRequest(id))
            {
                return RedirectToAction("AccountsChallenge", new { id = id,
                    returnTo = $"{_siteConnector.Services[SupportServices.Portal]}views/accounts/{id}" });
            }

            var paymentsView = await _siteConnector.DownloadView(SupportServices.Portal, $"resources/payments/accounts/{id}");
            var usersView = await _siteConnector.DownloadView(SupportServices.Portal, $"resources/users/accounts/{id}/");

            var accountDetailsViewModel = new AccountDetailViewModel
            {
                Account = _accountViewModels.Accounts.FirstOrDefault(x => x.AccountId == id),
                PaymentsView = paymentsView,
                UsersView = usersView
            };

            return View("_accountDetails", accountDetailsViewModel);
        }


        private bool ChallengeRequest(int id)
        {
            return MvcApplication.AccountChallenges
                       .FirstOrDefault(x =>
                           x.Value.AccountId == id
                           && x.Value.Identity == null
                           && x.Value.Until > DateTimeOffset.UtcNow).Value == null;
        }
        
        private void AddChallengePass(int id)
        {
            MvcApplication.AccountChallenges.AddOrUpdate(Guid.NewGuid(), new AgentAccountChallenge()
            {
                AccountId = id,
                Until = DateTimeOffset.UtcNow.AddSeconds(15)
            }, (guid, challenge) => { return challenge; });
            
        }

        public ActionResult AccountsChallenge(int id, string returnTo)
        {

            // TODO: Get Challenge for AccountId = id

            var model = new AccountsChallengeViewModel()
            {
                AccountId = id,
                Challenge = "Challenge",
                Response = null,
                ReturnTo = returnTo,
                ResponseUrl = $"{_siteConnector.Services[SupportServices.Accounts]}accounts/challenge/response"
            };
            return View("_accountsChallenge", model);
        }

        [Route("accounts/challenge/response")]
        [HttpPost]
        public ActionResult AccountsChallengeResponse(FormCollection formProperties)
        {
            var response = (formProperties ?? new FormCollection())["Response"] ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(response))
            {
                var id = (formProperties ?? new FormCollection())["AccountId"] ?? string.Empty;

                if (int.TryParse(id, out var accountId))
                {
                    // TODO: Verify response is valid for id

                    AddChallengePass(accountId);
                    var returnTo = (formProperties ?? new FormCollection())["ReturnTo"] ?? string.Empty;
                    return Redirect(returnTo);
                }
            }
            return Redirect("accounts/denied");
        }



        [Route("endcall/{identity?}")]
        public ActionResult EndCall(string identity)
        {

            MvcApplication.AccountChallenges.Clear();

            //var itemsToDelete = MvcApplication.AccountChallenges.Where(x => x.Value.Identity == identity).ToList();
            //foreach (var agentAccountChallenge in itemsToDelete)
            //{
                
            //}

            return View("EndCall");
        }

    }
}