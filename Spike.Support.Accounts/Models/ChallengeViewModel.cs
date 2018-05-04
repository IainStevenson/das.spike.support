namespace Spike.Support.Accounts.Models
{
    public class ChallengeViewModel
    {
        public int AccountId { get; set; }
        public string ReturnTo { get; set; }
        public string Challenge { get; set; }
        public string Response { get; set; }
        public string ResponseUrl { get; set; }
        public string EntityType { get; set; }
        public string Identity { get; set; }
        public int Tries { get; set; } = 1;
        public int MaxTries { get; set; } = 3;
        public string Message { get; set; }
    }
}