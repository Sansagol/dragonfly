using Dragonfly.Core.UserAccess;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Core
{
    /// <summary>
    /// Class handle all catched and unhandled exceptions.
    /// </summary>
    public class ControllersExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        Logger _Lg = null;
        public ControllersExceptionAttribute()
        {
            _Lg = LogManager.GetCurrentClassLogger();
        }

        /// <summary>Default errors handler.</summary>
        /// <param name="filterContext">Exception context.</param>
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                if (filterContext.Exception is AuthenticationException)
                    HandleAuthenticationException(filterContext);
                else
                {
                    _Lg.Error("{0}\n{1}",
                        filterContext.Exception.GetFullMessage(),
                        filterContext.Exception.GetStackTrace());
                    filterContext.Result = new RedirectResult("~/Content/CommonErrorPage.html");
                }
                filterContext.ExceptionHandled = true;
            }
        }

        private void HandleAuthenticationException(ExceptionContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult("Default", new System.Web.Routing.RouteValueDictionary()
            {
                {"controller", "Main" },
                {"action", "Authorization" }
            });
        }

        private void HandleauthorizationException()
        {
        }
    }
}