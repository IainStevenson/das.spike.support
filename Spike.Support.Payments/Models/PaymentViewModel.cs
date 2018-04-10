using System;
using System.ComponentModel.DataAnnotations;

namespace Spike.Support.Payments.Models
{
    public class PaymentViewModel
    {
        [Display(Name = "Account Id")] public int AccountId { get; set; }

        [Display(Name = "Payment Id")] public Guid PaymentId { get; set; }

        public DateTime Created { get; set; }
        public decimal Amount { get; set; }
    }
}