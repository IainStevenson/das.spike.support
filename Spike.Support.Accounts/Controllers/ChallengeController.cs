using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Spike.Support.Accounts.Models;
using Spike.Support.Shared.Communication;
using Spike.Support.Shared.Models;

namespace Spike.Support.Accounts.Controllers
{
    public class ChallengeController : Controller
    {
        private readonly ISiteConnector _siteConnector;

        public ChallengeController()
        {
            _siteConnector = new SiteConnector();
        }
        private readonly int _maxChallengeTries = 3;
        private string _menuType;

        private string GetChallengeForEntityType(string entityType)
        {
            return $"Challenge for {entityType} ";
        }

        [Route("accounts/challenge/{challengeId:guid}")]
        public ActionResult Challenge(Guid challengeId)
        {
            var formPostbackUrl = new Uri(_siteConnector.Services[SupportServices.Accounts],
                "accounts/challenge/response").AbsoluteUri;

            if (!MvcApplication.Challenges.ContainsKey(challengeId))
               return  RedirectToAction("Failed", "Challenge");

            var model = MvcApplication.Challenges[challengeId];



            model.Challenge = GetChallengeForEntityType(model.EntityType);
            model.MaxTries = _maxChallengeTries;
            model.ResponseUrl = formPostbackUrl;


            if (NavItem.IsAResourceRequest(Request))
                return View(model);

            var identifiers = new Dictionary<string, string> { { "accountId", $"{model.Identifier}" } };

            var navItems = MvcApplication.NavItems
                .Where(x => x.Key.StartsWith($"{model.MenuType}"))
                .ToDictionary(x => x.Key, x => x.Value);

            var menuNavItems = NavItem.TransformNavItems(
                    navItems,
                    _siteConnector.Services[SupportServices.Portal],
                    identifiers
                ).Select(s => s.Value)
                .ToList();


            ViewBag.Menu = Menu.ConfigureMenu(
                menuNavItems,
                model.EntityType,
                new List<string> { model.EntityType
                },
                MenuOrientations.Vertical);

            return View(model);
        }
        /// <summary>
        /// Processes the response, if its nothing or wrong the tries is incremented and passed back
        /// If Tries > MaxTries then the challenge is abanondeoned and passed back to the portals Challenge failed route.
        /// </summary>
        /// <param name="formProperties">The form properties collection posted back from the form on the Portal site</param>
        /// <returns>A redirect to the original request on success, a redirect to ask again of not exceeded retries, else a redirect to access denied</returns>
        [Route("accounts/challenge/response")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ProcessResponse(FormCollection formProperties)
        {
            if (formProperties == null) throw new ArgumentNullException(nameof(formProperties));

            var challengeId = new Guid(formProperties["ChallengeId"] ?? Guid.Empty.ToString());

            if (!MvcApplication.Challenges.ContainsKey(challengeId))
                return RedirectToAction("Failed", "Challenge");

            var model = MvcApplication.Challenges[challengeId];

            var response = formProperties["Response"];

            if (string.IsNullOrWhiteSpace(response) ||
                response.StartsWith("f", StringComparison.InvariantCultureIgnoreCase))
            {
                model.Tries += 1;
                if (model.Tries > model.MaxTries)
                {
                    return RedirectToAction("Failed", "Challenge");
                }
                model.Message = $"Please try again";
                return ReturnToChallenge(model);
            }

            await _siteConnector.Challenge(
                    $"api/challenge/passed/{model.EntityType}/{model.Identifier}/{model.Identity}");
            MvcApplication.Challenges.Remove(model.ChallengeId);
            return Redirect(model.ReturnTo);
        }


        private ActionResult ReturnToChallenge(ChallengeViewModel model)
        {
            var uri = $"views/accounts/challenge/{model.ChallengeId}";
            var redirectUri = new Uri(_siteConnector.Services[SupportServices.Portal], uri);
            return Redirect(redirectUri.AbsoluteUri);
        }

        public ActionResult Failed()
        {
            return View();
        }
    }
}