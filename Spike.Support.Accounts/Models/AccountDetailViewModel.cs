using System.Web.Mvc;

namespace Spike.Support.Accounts.Models
{
    public class AccountDetailViewModel
    {
        public AccountViewModel Account { get; set; }

        public MvcHtmlString View { get; set; }
        
    }

    public class AccountUsersViewModel
    {
        public AccountViewModel Account { get; set; }

        public MvcHtmlString View { get; set; }

    }

    public class AccountPaymentsViewModel
    {
        public AccountViewModel Account { get; set; }

        public MvcHtmlString View { get; set; }

    }
}