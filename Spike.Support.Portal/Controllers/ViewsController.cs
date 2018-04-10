using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Portal.Models;
using Spike.Support.Shared;

namespace Spike.Support.Portal.Controllers
{
    public class ViewsController : Controller
    {
        private readonly ISiteConnector _siteConnector;
        private readonly string _accountsBaseAddress = "https://localhost:44317/";
        private readonly string _usersBaseAddress = "https://localhost:44309/";

        public ViewsController()
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
            var indexViewModel = new IndexViewModel
            {
                UsersView = usersView,
                AccountsView = accountsView
            };


            return View("index", indexViewModel);
        }

        public async Task<ActionResult> Challenge(string id, string redirectTo)
        {
            var challengeViewModel = new ChallengeViewModel() { };

            return View("Challenge", challengeViewModel);
        }
        [HttpPost]
        public async Task<RedirectToRouteResult> Challenge(string id, string resourcId,  string redirectTo, string responseId)
        {
            if (id != responseId) return RedirectToAction("Challenge", new {id = id, redirectTo = redirectTo});

            await _siteConnector.Submit(new Uri(_usersBaseAddress), $"payments/authorise/{responseId}", null);
            if (_siteConnector.Reason == null)
            {
                return RedirectToAction("Index", new { path = redirectTo });
            }
            return RedirectToAction("Challenge", new { id = id, redirectTo = redirectTo });
        }

        [Route("views/{*path}")]
        public async Task<ActionResult> Index(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return new HttpNotFoundResult();

            var indexViewModel = new IndexViewModel();

            if (path.ToLower().StartsWith("users".ToLower()))
            {
                var mvcHtmlString = await _siteConnector.DownloadView(_usersBaseAddress, $"{path}");
                if ((_siteConnector.Reason ?? string.Empty).StartsWith("Challenge"))
                {
                    var reasons = _siteConnector.Reason.Split(':');
                    var resourceKey = reasons.Skip(1).First();
                    var resourceId = reasons.Skip(2).First();

                    return RedirectToAction("Challenge", new {id = $"{resourceId}", resource = $"{resourceKey}", redirect = $"views/{path}" });
                }
                indexViewModel.UsersView = mvcHtmlString;
            }
            if (path.ToLower().StartsWith("accounts".ToLower()))
                indexViewModel.AccountsView = await _siteConnector.DownloadView(_accountsBaseAddress, $"{path}");


            return View("index", indexViewModel);
        }
    }
}