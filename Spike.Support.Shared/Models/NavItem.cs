using System;
using System.Collections.Generic;
using System.Linq;

namespace Spike.Support.Shared.Models
{
    public class NavItem
    {
        public string Key { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string NavigateUrl { get; set; } = "/";
        public int Ordinal { get; set; }
        public string[] Roles { get; set; } = new string[] { };
        public List<NavItem> NavItems { get; set; } = new List<NavItem>();
        public static List<NavItem> TransformNavItems(
            Dictionary<string, NavItem> templates, Uri baseUrl,
            Dictionary<string, string> identifiers)
        {
            List<NavItem> menuItems = new List<NavItem>();

            foreach (var template in templates)
            {
                var item = new NavItem
                {
                    Key = template.Key,
                    Text = template.Value.Text,
                    NavigateUrl = new Uri(baseUrl, template.Value.NavigateUrl).OriginalString
                };
                foreach (var identifier in identifiers)
                {
                    item.NavigateUrl = item.NavigateUrl
                        .Replace($"{{{identifier.Key.Split('.').LastOrDefault() ?? string.Empty}}}", 
                            identifier.Value);
                }
                menuItems.Add(item);
            }
            return menuItems;
        }
    }
}
