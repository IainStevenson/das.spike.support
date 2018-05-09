using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Spike.Support.Portal.Models;
using Spike.Support.Shared.Communication;

namespace Spike.Support.Portal.Controllers
{
    public class ChallengeController : ApiController
    {
        private readonly short _challengeTimeoutMinutes = 1;
        private readonly IIdentityHandler _identityHandler;
        private readonly string _cookieName = "IdentityContextCookie";
        private readonly string _cookieDomain = ".localhost";  // change to real domain for deployment // change to real domain for deployment
        private readonly string _defaultIdentity = "anonymous";
        private string _identity;
        private string[] _roles;

        public ChallengeController()
        {
            _identityHandler = new CookieIdentityHandler(_cookieName, _cookieDomain, _defaultIdentity);
        }
        
        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            _identity = _identityHandler.GetIdentity(new HttpRequestWrapper(HttpContext.Current.Request));
            Debug.WriteLine($"App-Debug: {(nameof(ChallengeController))} (API) {nameof(ExecuteAsync)} Recieves Identity {_identity}");

            return base.ExecuteAsync(controllerContext, cancellationToken);
        }

        [HttpGet]
        [Route("api/challenge/required/{entityType}/{identifier}")] // 
        public async Task<bool> Required(string entityType, string identifier)
        {

            Debug.WriteLine($"App-Debug: {(nameof(ChallengeController))} {nameof(Required)} {entityType}, {identifier}");

            var item = MvcApplication.SupportAgentChallenges.Values.FirstOrDefault(x =>
                x.EntityType == entityType
                && x.Identifier == identifier
                && x.Identity == _identity
                && x.Until > DateTimeOffset.UtcNow);

            return await Task.FromResult(item == null);
        }
        
        [HttpGet]
        [Route("api/challenge/passed/{entityType}/{identifier}")]
        public async Task<bool> Passed(string entityType, string identifier)
        {
            Debug.WriteLine($"App-Debug: {(nameof(ChallengeController))} {nameof(Passed)} {entityType}, {identifier}");
            var item = new SupportAgentChallenge
            {
                EntityType = entityType,
                Identifier = identifier,
                Identity = _identity,
                Until = DateTimeOffset.UtcNow.AddMinutes(_challengeTimeoutMinutes)
            };
            MvcApplication.SupportAgentChallenges.Add(item.Id, item);
            return await Task.FromResult(true);
        }
        [HttpGet]
        [Route("api/challenge/refresh/{entityType}/{identifier}")]
        public async Task<bool> Refresh(string entityType, string identifier)
        {
            Debug.WriteLine($"App-Debug: {(nameof(ChallengeController))} {nameof(Refresh)} {entityType}, {identifier}");

            var item = MvcApplication.SupportAgentChallenges
                    .FirstOrDefault(x => x.Value.EntityType == entityType 
                                         && x.Value.Identifier == identifier 
                                         && x.Value.Identity == _identity);
            if (item.Value == null) return await Task.FromResult(true);
            var challenge = item.Value;
            challenge.Until = DateTimeOffset.UtcNow.AddMinutes(_challengeTimeoutMinutes);

            return await Task.FromResult(true);
        }
        [HttpGet]
        [Route("api/challenge/clear")]
        public async Task<bool> Clear()
        {
            Debug.WriteLine($"App-Debug: {(nameof(ChallengeController))} {nameof(Clear)}");
            var itemList = MvcApplication.SupportAgentChallenges.Where(
                f => f.Value.Identity.Equals(_identity, StringComparison.InvariantCultureIgnoreCase)
            ).Select(x => x.Key).ToList();

            foreach (var id in itemList) MvcApplication.SupportAgentChallenges.Remove(id);
            return await Task.FromResult(true);
        }
    }
}