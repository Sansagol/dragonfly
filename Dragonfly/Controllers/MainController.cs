using Dragonfly.Core;
using Dragonfly.Core.UserAccess;
using Dragonfly.Database;
using Dragonfly.Database.Providers;
using Dragonfly.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    public class MainController : Controller
    {
        Logger _Lg = LogManager.GetCurrentClassLogger();
        private IUserAuthenticateStateManager _UserStateManager = null;

        /// <summary>
        /// The default constructor for using in the app.
        /// </summary>
        public MainController()
        {
            try
            {
                _UserStateManager = BaseBindings.UsrStateManager;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullMessage());
                throw ex;
            }
        }

        /// <summary>The constructor for using in tests.</summary>
        /// <param name="usrStateMngr">Custom user state manager.</param>
        public MainController(IUserAuthenticateStateManager usrStateMngr)
        {
            if (usrStateMngr == null)
                throw new ArgumentNullException(nameof(usrStateMngr));
            _UserStateManager = usrStateMngr;
        }

        // GET: Main
        [HttpGet]
        [ControllersException]
        public ActionResult Index()
        {
            ViewBag.Logged = false;
            if (_UserStateManager.CheckUserAccess(Request, Response))
            {
                ViewBag.Logged = true;
                ViewBag.UserName = BaseBindings.CookiesManager.GetCookieValue(
                    Request,
                    CookieType.UserName);
            }
            return View();
        }

        [HttpGet]
        public ViewResult LogIn()
        {
            return View(new AuthenticateModel());
        }

        /// <summary>Method try to authorize user in the system.</summary>
        /// <param name="authParameters">Auth parameters.</param>
        /// <returns>Reload page (if error), or return to main.</returns>
        [HttpPost]
        public ActionResult LogIn(AuthenticateModel authParameters)
        {
            var cookMan = BaseBindings.CookiesManager;
            if (ModelState.IsValid)
            {
                if (_UserStateManager.LogIn(Response, authParameters))
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(authParameters);
        }

        public ActionResult LogOut()
        {
            _UserStateManager.LogOut(Request, Response);
            return RedirectToAction(nameof(Index));
        }

#if DEBUG
        /// <summary>Must invoke manually.</summary>
        [ControllersException]
        public void ErrFun()
        {
            throw new Exception("Err");
        }
#endif
    }
}