using System;
using System.ComponentModel.DataAnnotations;

namespace Spike.Support.Users.Models
{
    public class UserViewModel
    {
        [Display(Name ="User Id")]
        public int UserId { get; set; }
        [Display(Name ="Account Id")]
        public int AccountId { get; set; }
        [Display(Name ="User Name")]
        public string UserName { get; set; }
        public DateTime Created { get; set; }
    }
}