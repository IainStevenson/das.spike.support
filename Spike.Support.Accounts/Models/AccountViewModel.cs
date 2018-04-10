using System;
using System.ComponentModel.DataAnnotations;

namespace Spike.Support.Accounts.Models
{
    public class AccountViewModel
    {
        [Display(Name = "Account Id")] public int AccountId { get; set; }

        [Display(Name = "Account Name")] public string AccountName { get; set; }

        public DateTime Created { get; set; }
    }
}