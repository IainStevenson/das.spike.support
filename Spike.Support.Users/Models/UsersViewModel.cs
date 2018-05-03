using System.Collections.Generic;
using System.Web.Mvc;

namespace Spike.Support.Users.Models
{
    public class UsersViewModel
    {
        public List<UserViewModel> Users { get; set; }
    }


    public class UserAccountsViewModel
    {
        public UserViewModel User { get; set; }
        public MvcHtmlString View { get; set; }
    }
}