using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Spike.Support.Payments.Models;
using Spike.Support.Shared;
using Spike.Support.Shared.Communication;
using Spike.Support.Shared.Models;

namespace Spike.Support.Payments.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly PaymentsViewModel _paymentsViewModels;
        private readonly ISiteConnector _siteConnector;

        public PaymentsController()
        {
            _siteConnector = new SiteConnector();
            _paymentsViewModels = new PaymentsViewModel
            {
                Payments = Enumerable.Range(1, 1000).Select(x =>
                    new PaymentViewModel
                    {
                        AccountId = x % 100,
                        PaymentId = x,
                        Created = DateTimeOffset.UtcNow.AddDays(-x * 7),
                        Direction = x % 3 == 0 ? "In" : "Out",
                        Amount = x % 3 == 0 ? x * 1000 : x * -1000
                    }).ToList()
            };
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
        [Route("payments")]
        public ActionResult Index()
        {
            return View("payments", _paymentsViewModels);
        }

        [Route("payments/{id:guid}")]
        public ActionResult Index(int id)
        {
            var paymentsViewModel = new List<PaymentViewModel>
            {
                _paymentsViewModels.Payments.FirstOrDefault(x => x.PaymentId == id)
            };
            return View("payments", new PaymentsViewModel
            {
                Payments = paymentsViewModel
            });
        }

        [Route("payments/accounts/{accountId:int}/{direction?}")]
        public ActionResult AccountPayments(int accountId, string direction = null)
        {
            var paymentsViewModel = new PaymentsViewModel
            {
                Payments = _paymentsViewModels.Payments
                    .Where(x => x.AccountId == accountId && x.Direction.Equals(direction ?? x.Direction,
                                    StringComparison.InvariantCultureIgnoreCase))
                    .ToList()
            };
            return View("_accountPaymentDetails", paymentsViewModel);
        }
    }
}