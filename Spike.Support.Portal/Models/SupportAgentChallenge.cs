using System;

namespace Spike.Support.Portal.Models
{
    public class SupportAgentChallenge
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EntityType { get; set; }
        public string Identifier { get; set; }
        public string Identity { get; set; }
        public DateTimeOffset Until { get; set; }
    }
}