using System;

namespace Spike.Support.Accounts
{
    public class AgentAccountChallenge
    {
        public int AccountId { get; set; }
        public string Identity { get; set; }
        public DateTimeOffset Until { get; set; }
    }
}