using System.Collections.Generic;
using System.Web.Mvc;

namespace Spike.Support.Portal.Models
{
    public class EndCallViewModel
    {
        public List<MvcHtmlString> ServiceViews { get; set; } = new List<MvcHtmlString>();
    }
}