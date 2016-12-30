using Dragonfly.Core;
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
        public ActionResult Index()
        {
            if (!UserStateManager.IsUserLogged)
            {
                ViewBag.Greeting = "Please log in";
                ViewBag.Logged = false;
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

        [HttpPost]
        public ActionResult Authorization(AuthenticateModel authParameters)
        {
            UserModel user = null;
            if (!string.IsNullOrWhiteSpace(authParameters.Login) &&
                !string.IsNullOrWhiteSpace(authParameters.Password))
            {
                UserStateManager.IsUserLogged = true;
                UserStateManager.UserName = authParameters.Login;
                user = new UserModel() { Login = authParameters.Login };
            }
            else
                UserStateManager.IsUserLogged = false;
            return RedirectToAction("Index");
        }
    }
}