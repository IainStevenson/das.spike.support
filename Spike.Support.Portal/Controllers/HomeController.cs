using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Portal.Models;

namespace Spike.Support.Portal.Controllers
{
    public class HomeController : Controller
    {
        private string _usersBaseAddress = "https://localhost:44309/";
        private string _accountsBaseAddress = "https://localhost:44317/";
        private readonly ISiteConnector _siteConnector;

        public HomeController()
        {
            _siteConnector = new SiteConnector();
        }

        [Route("")]
        public async Task<ActionResult> Index()
        {

            // OK.... here we go.
            // Make download calls to Users, Accounts, Accounts will ask for a Download roundtrip for Payements.. by Account
            var usersView = await _siteConnector.DownloadView(_usersBaseAddress, "users");
            var accountsView = await _siteConnector.DownloadView(_accountsBaseAddress, "accounts");
            var indexViewModel = new IndexViewModel()
            {
                UsersView = usersView,
                AccountsView = accountsView
            };


            return View("index", indexViewModel);
        }

        [Route("{view}/{id}")]
        public async Task<ActionResult> Index(string view , object id)
        {

            // OK.... here we go.
            // Make download calls to Users, Accounts, Accounts will ask for a Download roundtrip for Payements.. by Account
            var usersView = await _siteConnector.DownloadView(_usersBaseAddress, (view.Equals("users")? $"users/{id}": $"users"));
            var accountsView = await _siteConnector.DownloadView(_accountsBaseAddress, (view.Equals("accounts")? $"accounts/{id}": $"accounts"));
            var indexViewModel = new IndexViewModel()
            {
                UsersView = usersView,
                AccountsView = accountsView
            };
            indexViewModel.AccountsView = (view.Equals("accounts") ? accountsView : null);
            indexViewModel.UsersView = (view.Equals("users") ? usersView: null);

            return View("index", indexViewModel);
        }
    }
}