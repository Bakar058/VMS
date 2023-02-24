using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace VMS.App_Start
{
    public class UserAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Check if the user has a valid authentication cookie
            HttpCookie cookie = httpContext.Request.Cookies["UserCookie"];
            return cookie != null && cookie.Value == "LoggedIn";
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "User" },
                { "action", "Login" }
            });
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }


    public class AdminAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Check if the user has a valid authentication cookie
            HttpCookie cookie = httpContext.Request.Cookies["AdminCookie"];
            return cookie != null && cookie.Value == "LoggedIn";
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "Admin" },
                { "action", "Login" }
            });
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }




    public class SuperAdminAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Check if the user has a valid authentication cookie
            HttpCookie cookie = httpContext.Request.Cookies["SuperAdminCookie"];
            return cookie != null && cookie.Value == "LoggedIn";
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "SuperAdmin" },
                { "action", "Login" }
            });
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }

}
