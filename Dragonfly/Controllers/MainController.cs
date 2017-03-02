using Dragonfly.Core;
using Dragonfly.Database;
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

        // GET: Main
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                ViewBag.Logged = false;
                //return RedirectToAction(nameof(Authorization), new AuthenticateModel());
            }
            else
            {
                int userId = -1;
                if (int.TryParse(Session["UserId"].ToString(), out userId))
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
                            Session["UserName"] = user.Login;
                            Session["UserId"] = user.Id;
                            return RedirectToAction("Index");
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
                Session["UserId"] = null;
            return View(authParameters);
        }

        /// <summary>Method log out current user.</summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Logout()
        {
            Session["UserId"] = null;
            return RedirectToAction("Index");
        }
    }
}