using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Accounts.Models;
using Spike.Support.Shared;

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
        private readonly string _portalAddress = "https://localhost:44394/";


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
            var paymentsView = await _siteConnector.DownloadView(new Uri(_portalAddress),
                $"resources/resource/payments/accounts/{id}");
            var usersView =
                await _siteConnector.DownloadView(new Uri(_portalAddress), $"resources/resource/users/accounts/{id}/");

            var accountDetailsViewModel = new AccountDetailViewModel
            {
                Account = _accountViewModels.Accounts.FirstOrDefault(x => x.AccountId == id),
                PaymentsView = paymentsView,
                UsersView = usersView
            };

            return View("_accountDetails", accountDetailsViewModel);
        }
    }
}