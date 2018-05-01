using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Payments.Models;
using Spike.Support.Shared;

namespace Spike.Support.Payments.Controllers
{
    public class PaymentsController : ViewControllerBase
    {
        private readonly PaymentsViewModel _paymentsViewModels;
        

        public PaymentsController()
        {
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
        public async Task<ActionResult> EndCall(string identity)
        {
            return View("EndCall");
        }

    }
}