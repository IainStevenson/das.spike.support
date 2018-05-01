namespace Spike.Support.Accounts.Models
{
    public class AccountsChallengeViewModel
    {
        public int AccountId { get; set; }
        public string ReturnTo { get; set; }
        public string Challenge { get; set; }
        public string Response { get; set; }
        public string ResponseUrl { get; set; }
        public string EntityType { get; set; }
        public string Identity { get; set; }
    }
}