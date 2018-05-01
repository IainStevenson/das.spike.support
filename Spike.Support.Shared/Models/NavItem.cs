using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Spike.Support.Shared.Models
{
    public class NavItem
    {
        public string Text { get; set; } = string.Empty;
        public string NavigateUrl { get; set; } = "/";
        public int Ordinal { get; set; }

        public string[] Roles { get; set; } = new string[] { };
        public List<NavItem> NavItems { get; set; } = new List<NavItem>();


        public static List<NavItem> GetItems(Dictionary<string, string> templates, Dictionary<string, string> identifiers)
        {
            List<NavItem> menuItems = new List<NavItem>();

            foreach (var template in templates)
            {
                var item = new NavItem();
                item.Text = template.Key;
                var url = template.Value;
                foreach (var identifier in identifiers)
                {
                   url = url.Replace($"{{{identifier.Key.Split('.').LastOrDefault()??string.Empty}}}", identifier.Value);
                }
                item.NavigateUrl = url;
                menuItems.Add(item);
            }
            return  menuItems;
            
        }
    }
}
