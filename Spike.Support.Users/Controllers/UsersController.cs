﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Spike.Support.Users.Models;

namespace Spike.Support.Users.Controllers
{
    public class UsersController : Controller
    {
        private readonly UsersViewModel _usersViewModel = new UsersViewModel
        {
            Users = Enumerable.Range(1, 10).Select(x => new UserViewModel
            {
                UserId = x,
                AccountId = x,
                UserName = $"User {x}",
                Created = DateTime.Now.AddMonths(-x)
            }).ToList()
        };

        [Route("")]
        [Route("users")]
        public ActionResult Index()
        {
            return View("users", _usersViewModel);
        }

        [Route("users/{id:int}")]
        public ActionResult Index(int id)
        {
            return View("users", new UsersViewModel
            {
                Users = new List<UserViewModel>
                {
                    _usersViewModel.Users.FirstOrDefault(x => x.UserId == id)
                }
            });
        }
    }
}