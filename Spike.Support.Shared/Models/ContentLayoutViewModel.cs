using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Spike.Support.Shared.Models
{
    public class ContentLayoutViewModel
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public object Content { get; set; }
    }
}