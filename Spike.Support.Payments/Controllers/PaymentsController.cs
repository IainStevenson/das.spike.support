using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Payments.Models;

namespace Spike.Support.Payments.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly PaymentsViewModel _paymentsViewModels;
        private readonly string _authorisationChallengeResponse = "Challenge";
        private readonly Dictionary<string, DateTimeOffset> _authorisations = new Dictionary<string, DateTimeOffset>();

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
            if (!CheckAuhtorisation($"{id}")) return new HttpUnauthorizedResult($"{_authorisationChallengeResponse}:payments:{id}");

            var paymentsViewModel = new List<PaymentViewModel>
            {
                _paymentsViewModels.Payments.FirstOrDefault(x => x.PaymentId == id)
            };
            return View("payments", new PaymentsViewModel
            {
                Payments = paymentsViewModel
            });
        }

        private bool CheckAuhtorisation(string id)
        {
            if (!_authorisations.Keys.Contains(id)) return false;
            return _authorisations[id].AddMinutes(15) < DateTimeOffset.UtcNow;
        }

        [HttpPost]
        [Route("payments/authorise/{id}")]
        public void Authorise(string id)
        {
            _authorisations.Add(id, DateTimeOffset.UtcNow);
        }

        [Route("payments/accounts/{id:int}")]
        public ActionResult AccountPayments(int id)
        {
            if (!CheckAuhtorisation($"{id}")) return new HttpUnauthorizedResult($"{_authorisationChallengeResponse}:payments:{id}");

            var paymentsViewModel = new PaymentsViewModel
            {
                Payments = _paymentsViewModels.Payments.Where(x => x.AccountId == id).ToList()
            };
            return View("_accountPaymentDetails", paymentsViewModel);
        }
    }
}