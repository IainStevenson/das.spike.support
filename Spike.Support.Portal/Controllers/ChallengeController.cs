﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Spike.Support.Portal.Models;

namespace Spike.Support.Portal.Controllers
{
    public class ChallengeController : ApiController
    {
        private readonly short _challengeTimeoutMinutes = 1;
        [HttpGet]
        [Route("api/challenge/required/{entityType}/{identifier}/{identity}")] // 
        public async Task<bool> Required(string entityType, string identifier, string identity)
        {
            var item = MvcApplication.SupportAgentChallenges.Values.FirstOrDefault(x =>
                x.EntityType == entityType
                && x.Identifier == identifier
                && x.Identity == identity
                && x.Until > DateTimeOffset.UtcNow);
            return await Task.FromResult(item == null);
        }

        [HttpGet]
        [Route("api/challenge/passed/{entityType}/{identifier}/{identity}")]
        public async Task<bool> Passed(string entityType, string identifier, string identity)
        {
            var item = new SupportAgentChallenge
            {
                EntityType = entityType,
                Identifier = identifier,
                Identity = identity,
                Until = DateTimeOffset.UtcNow.AddMinutes(_challengeTimeoutMinutes)
            };
            MvcApplication.SupportAgentChallenges.Add(item.Id, item);
            return await Task.FromResult(true);
        }

        [HttpGet]
        [Route("api/challenge/clear/{identity}")]
        public async Task<bool> EndCall(string identity)
        {
            var itemList = MvcApplication.SupportAgentChallenges.Where(
                f => f.Value.Identity == identity
            ).Select(x => x.Key).ToList();

            foreach (var id in itemList) MvcApplication.SupportAgentChallenges.Remove(id);
            return await Task.FromResult(true);
        }
    }
}