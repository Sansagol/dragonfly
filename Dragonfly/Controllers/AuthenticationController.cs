using Dragonfly.Core;
using Dragonfly.Database.Providers;
using Dragonfly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet]
        public ViewResult Login()
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
                            return RedirectToAction(nameof(Login));
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
            return View("Index", "Main", authParameters);
        }
    }
}