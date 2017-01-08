﻿using Dragonfly.Core;
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
            if (!UserStateManager.IsUserLogged)
            {
                ViewBag.Greeting = "Please log in";
                ViewBag.Logged = false;
                return RedirectToAction(nameof(Authorization));
            }
            else
            {
                ViewBag.Greeting = $"Hello, {UserStateManager.UserName}";
                ViewBag.Logged = true;
            }
            return View();
        }

        [HttpGet]
        public ViewResult Authorization()
        {
            return View();
        }

        /// <summary>Method try to authorize user in the system.</summary>
        /// <param name="authParameters">Auth parameters.</param>
        /// <returns>Reload page (if error), or return to main.</returns>
        [HttpPost]
        public ActionResult Authorization(AuthenticateModel authParameters)
        {
            if (ModelState.IsValid)
            {
                UserStateManager.IsUserLogged = authParameters.CheckUser();
                if (UserStateManager.IsUserLogged)
                {
                    UserStateManager.UserName = authParameters.Login;
                    return RedirectToAction("Index");
                }
            }
            else
                UserStateManager.IsUserLogged = false;
            return View();
        }
    }
}