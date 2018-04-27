using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Portal.Models;
using Spike.Support.Shared;
using Spike.Support.Shared.Communication;

namespace Spike.Support.Portal.Controllers
{
    public class ViewsController : Controller
    {
        private readonly ISiteConnector _siteConnector;
       
        public ViewsController()
        {
            _siteConnector = new SiteConnector();
        }

        [Route("")]
        public async Task<ActionResult> Index()
        {
            // OK.... here we go.
            // Make download calls to Users, Accounts, Accounts will ask for a Download roundtrip for Payements.. by Account
            var usersView = await _siteConnector.DownloadView(SupportServices.Users, "users");
            var accountsView = await _siteConnector.DownloadView(SupportServices.Accounts, "accounts");
            var indexViewModel = new IndexViewModel
            {
                UsersView = usersView,
                AccountsView = accountsView
            };


            return View("index", indexViewModel);
        }

        [Route("views/{*path}")]
        public async Task<ActionResult> Index(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return new HttpNotFoundResult();

            var indexViewModel = new IndexViewModel();

            if (path.ToLower().StartsWith("users".ToLower()))
                indexViewModel.UsersView = await _siteConnector.DownloadView(SupportServices.Users, $"{path}");
            if (path.ToLower().StartsWith("accounts".ToLower()))
                indexViewModel.AccountsView = await _siteConnector.DownloadView(SupportServices.Accounts, $"{path}");
            return View("index", indexViewModel);
        }


        [Route("endcall/{identity?}")]
        public async Task<ActionResult> EndCall(string identity)
        {
            // call all of the sites to clear call authorisation
            var model = new EndCallViewModel();
            foreach (SupportServices supportService in _siteConnector.Services.Keys.Where(x=>x != SupportServices.Portal))
            {
                model.ServiceViews.Add(await _siteConnector.DownloadView(supportService, $"endcall"));
            }

            return View("EndCall", model);
        }
    }
}