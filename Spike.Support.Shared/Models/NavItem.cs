using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Spike.Support.Shared.Models
{
    public class NavItem
    {
        public string Key { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string NavigateUrl { get; set; } = "/";
        public int Ordinal { get; set; }
        public string[] Roles { get; set; } = { };
        public List<NavItem> NavItems { get; set; } = new List<NavItem>();

        public static bool IsAResourceRequest(HttpRequestBase request)
        {
            return request.Headers.AllKeys.Contains("X-Resource");
        }

        public static Dictionary<string, NavItem> TransformNavItems(
            Dictionary<string, NavItem> templates, Uri baseUrl,
            Dictionary<string, string> identifiers)
        {
            var templateCopies = JsonConvert.DeserializeObject<Dictionary<string, NavItem>>(
                JsonConvert.SerializeObject(templates)
            );

            var menuItems = templateCopies
                .Select(i => i.Value)
                .Map(s => true, n => n.NavItems).ToList();

            foreach (var menuItem in menuItems)
            foreach (var identifier in identifiers)
                menuItem.NavigateUrl =
                    new Uri(baseUrl, menuItem.NavigateUrl)
                        .OriginalString
                        .Replace($"{{{identifier.Key.Split('.').LastOrDefault() ?? string.Empty}}}",
                            identifier.Value);
            //// remove navigations with untransformed variables.
            //foreach (var menuItem in menuItems.ToList())
            //{
            //    if (menuItem.NavigateUrl.Contains("/{"))
            //    {
            //        menuItems.Remove(menuItem);
            //    }
            //}

            return templateCopies;
        }
    }
}