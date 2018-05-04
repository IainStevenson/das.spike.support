using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Spike.Support.Portal.Controllers
{
    [RoutePrefix("views/challenges")]
    public class ChallengesController : Controller
    {


        // GET: Challenges
        public ActionResult Denied()
        {
            return View();
        }
    }
}