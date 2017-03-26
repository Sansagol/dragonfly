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
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                filterContext.Result = new RedirectResult("~/Content/CommonErrorPage.html");
                filterContext.ExceptionHandled = true;
            }
        }
    }
}