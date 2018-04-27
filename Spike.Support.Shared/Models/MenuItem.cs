namespace Spike.Support.Shared.Models
{
    public class MenuItem
    {
        public string Text { get; set; }
        public string Url { get; set; }

        public int Ordinal { get; set; }
        public string[] Roles { get; set; } = new string[] { };
    }
}