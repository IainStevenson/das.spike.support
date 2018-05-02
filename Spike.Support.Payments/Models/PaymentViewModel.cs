using System;
using System.ComponentModel.DataAnnotations;

namespace Spike.Support.Payments.Models
{
    public class PaymentViewModel
    {
        [Display(Name = "Account Id")] public int AccountId { get; set; }

        [Display(Name = "Payment Id")] public int PaymentId { get; set; }

        public DateTimeOffset Created { get; set; }
        public decimal Amount { get; set; }
        public string Direction { get; set; }
    }
}