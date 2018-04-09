using System.Web.Mvc;

namespace Spike.Support.Accounts.Models
{
    public class AccountDetailViewModel
    {
        public AccountViewModel Account { get; set; }

        public MvcHtmlString PaymentsView { get; set; }
    }
}