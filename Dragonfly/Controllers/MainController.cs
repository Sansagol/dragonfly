﻿using Dragonfly.Core;
using Dragonfly.Database;
using Dragonfly.Database.Providers;
using Dragonfly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    public class MainController : Controller
    {
        private IUserStateManager _UserStateManager = null;

        /// <summary>
        /// The default constructor for using in the app.
        /// </summary>
        public MainController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
        }

        /// <summary>The constructor for using in tests.</summary>
        /// <param name="usrStateMngr">Custom user state manager.</param>
        public MainController(IUserStateManager usrStateMngr)
        {
            if (usrStateMngr == null)
                throw new ArgumentNullException(nameof(usrStateMngr));
            _UserStateManager = usrStateMngr;
        }

        // GET: Main
        [HttpGet]
        public ActionResult Index()
        {
            if (!_UserStateManager.CheckUserAccess(Request))
            {
                Logout();
                ViewBag.Logged = false;
                //return RedirectToAction(nameof(Authorization), new AuthenticateModel());
            }
            else
            {
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
                    Logout();
                    ViewBag.Logged = false;
                }
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
                    using (IDataBaseProvider provider = BaseBindings.GetNewBaseDbProvider())
                    {
                        UserModel user = provider.GetUserByLoginMail(authParameters.Login);
                        if (user != null)
                        {
                            using (var userAccessProvider = BaseBindings.GetNewUserAccessProvider())
                            {
                                string accToken = userAccessProvider.CreateAccessToken(user.Id);
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


        /// <summary>Method log out current user.</summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Logout()
        {
            var cookMan = BaseBindings.CookiesManager;
            string token = cookMan.GetCookieValue(Request, CookieType.UserAccessToken);
            if (!string.IsNullOrWhiteSpace(token))
                using (var ap = BaseBindings.GetNewUserAccessProvider())
                {
                    ap.DeleteAccessToken(token);
                }

            cookMan.DeleteCookie(Response, CookieType.UserAccessToken);
            cookMan.DeleteCookie(Response, CookieType.UserId);

            return RedirectToAction(nameof(Index));
        }
    }
}