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

       
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!MvcApplication.NavItems.Any())
            {
                MvcApplication.NavItems = _siteConnector.GetMenuTemplates<Dictionary<string, NavItem>>(
                    SupportServices.Portal, 
                    "api/navigation/payment") ?? new Dictionary<string, NavItem>();

            }
            base.OnActionExecuting(filterContext);
        }

        public PaymentsController()
        {
            _siteConnector = new SiteConnector();
            _paymentsViewModels = new PaymentsViewModel
            {
                Payments = Enumerable.Range(1, 1000).Select(x =>
                    new PaymentViewModel
                    {
                        AccountId = x % 100,
                        PaymentId = Guid.NewGuid(),
                        Created = DateTime.Now.AddMonths(-x),
                        Amount = x * 1000
                    }).ToList()
            };
            
        }

        [Route("")]
        [Route("payments")]
        public ActionResult Index()
        {
            return View("payments", _paymentsViewModels);
        }

        [Route("payments/{id:guid}")]
        public ActionResult Index(Guid id)
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

        [Route("payments/accounts/{accountId:int}")]
        public ActionResult AccountPayments(int accountId)
        {
            var paymentsViewModel = new PaymentsViewModel
            {
                Payments = _paymentsViewModels.Payments.Where(x => x.AccountId == accountId).ToList()
            };
            return View("_accountPaymentDetails", paymentsViewModel);
        }


        [Route("endcall/{identity?}")]
        public ActionResult EndCall(string identity)
        {
            return View("EndCall");
        }

    }
}