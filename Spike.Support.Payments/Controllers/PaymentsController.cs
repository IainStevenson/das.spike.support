using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Spike.Support.Payments.Models;

namespace Spike.Support.Payments.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly PaymentsViewModel _paymentsViewModels ;


        public PaymentsController()
        {
            _paymentsViewModels = new PaymentsViewModel()
            {
                Payments = Enumerable.Range(1, 1000).Select(x => 
                    new PaymentViewModel { AccountId = x % 100, PaymentId = Guid.NewGuid(), Created = DateTime.Now.AddMonths(-x), Amount = (x * 1000) }).ToList()
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
            return View("payments", new PaymentsViewModel()
            {
                Payments = new List<PaymentViewModel>()
                {
                    _paymentsViewModels.Payments.FirstOrDefault(x => x.PaymentId == id)
                }
            });
        }

        [Route("payments/accounts/{id:int}")]
        public ActionResult AccountPayments(int id)
        {
            return View("_accountPaymentDetails", new PaymentsViewModel()
            {
                Payments = _paymentsViewModels.Payments.Where(x => x.AccountId == id).ToList()
            });
        }
    }
}