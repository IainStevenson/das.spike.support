using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Spike.Support.Shared.Communication;

namespace Spike.Support.Shared
{
    public abstract class ViewControllerBase : Controller
    {
        protected Dictionary<string, string> _menuItems = new Dictionary<string, string>();
        protected readonly ISiteConnector _siteConnector;

        public ViewControllerBase()
        {
            _siteConnector = new SiteConnector();
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_menuItems.Any())
            {
        //        _menuItems = _siteConnector.DownloadResources<Dictionary<string,string>>(SupportServices.Portal, "api/navigation/accounts").Result;
                
            }
            base.OnActionExecuting(filterContext);
        }
    }
}