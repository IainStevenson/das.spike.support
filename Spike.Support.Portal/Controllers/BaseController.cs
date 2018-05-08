using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Spike.Support.Portal.Controllers
{
    public abstract class BaseController : Controller
    {
        protected static string _identityContextCookieName = "IdentityContextCookie";

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {

            base.OnResultExecuted(filterContext);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (!filterContext.HttpContext.Response.Cookies.AllKeys.Contains(_identityContextCookieName))
            //{
            //    var cookie = new HttpCookie(_identityContextCookieName, "test@user@domain.com")
            //    {
            //        HttpOnly = true,
            //        Secure = true,
            //        Domain = $".localhost"
            //    };
            //    filterContext.HttpContext.Response.Cookies.Add(cookie);
            //}

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
    }
}