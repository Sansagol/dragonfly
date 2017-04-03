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
                _Lg.Error("{0}\n{1}",
                    filterContext.Exception.GetFullMessage(),
                    filterContext.Exception.GetStackTrace());
                filterContext.Result = new RedirectResult("~/Content/CommonErrorPage.html");
                filterContext.ExceptionHandled = true;
            }
        }
    }
}