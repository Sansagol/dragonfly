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
            _UserStateManager.CheckUserAccess(Request, Response);

            int userId = -1;
            if (int.TryParse(BaseBindings.CookiesManager.GetCookieValue(Request, CookieType.UserId), out userId))
            {
                using (IDataBaseProvider provider = BaseBindings.GetNewBaseDbProvider())
                {
                    UserModel user = provider.GetUserById(userId);
                    if (user != null)
                    {
                        ViewBag.Logged = true;
                        ViewBag.UserName = user.Login;
                    }
                }
            }
            else
            {
                _UserStateManager.LogOut(Request, Response);
            }

            return View();
        }

        [HttpGet]
        public ViewResult Authorization()
        {
            return View(new AuthenticateModel());
        }

        /// <summary>Method try to authorize user in the system.</summary>
        /// <param name="authParameters">Auth parameters.</param>
        /// <returns>Reload page (if error), or return to main.</returns>
        [HttpPost]
        public ActionResult Authorization(AuthenticateModel authParameters)
        {
            var cookMan = BaseBindings.CookiesManager;
            if (ModelState.IsValid)
            {
                bool isTrueUser = authParameters.CheckUser();
                if (isTrueUser)
                {
                    using (IDataBaseProvider provider = BaseBindings.DBFactory.CreateDBProvider(
                        BaseBindings.SettingsReader.GetDbAccessSettings()))
                    {
                        UserModel user = provider.GetUserByLoginMail(authParameters.Login);
                        if (user != null)
                        {
                            using (var ap = BaseBindings.DBFactory.CreateUserAccessProvider(
                                BaseBindings.SettingsReader.GetDbAccessSettings()))
                            {
                                string accToken = ap.CreateAccessToken(user.Id);
                                cookMan.SetToCookie(Response, CookieType.UserAccessToken, accToken);
                                cookMan.SetToCookie(Response, CookieType.UserId, user.Id.ToString());
                            }
                            Session["UserName"] = user.Login;
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                else
                {
                    authParameters.IsTrueUser = false;
                    authParameters.ErrorOnUserChecking = "User not found";
                }
            }
            else
            {
                cookMan.DeleteCookie(Response, CookieType.UserId);
            }
            return View(authParameters);
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