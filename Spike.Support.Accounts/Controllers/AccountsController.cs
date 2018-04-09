using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Spike.Support.Accounts.Models;

namespace Spike.Support.Accounts.Controllers
{
    public class AccountsController : Controller
    {
        private readonly AccountsViewModel _accountViewModels =  new AccountsViewModel()
        {
            Accounts = Enumerable.Range(1, 10).Select(x => 
                new AccountViewModel { AccountId = x,
                    Created = DateTime.Now.AddMonths(-x),
                    AccountName = $"Account {x}" }).ToList()
        };


        [Route("")]
        [Route("accounts")]
        public ActionResult Index()
        {
            return View("accounts", _accountViewModels);
        }

        [Route("accounts/{id:int}")]
        public ActionResult Index(int id)
        {
            return View("accounts", new AccountsViewModel(){ Accounts =  new List<AccountViewModel>() { _accountViewModels.Accounts.FirstOrDefault(x => x.AccountId == id) } });
        }

    }
}