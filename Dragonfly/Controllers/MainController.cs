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

        [HttpPost]
        public ActionResult Authorization(AuthenticateModel authParameters)
        {
            UserModel user = null;
            if (ModelState.IsValid)
            {


                UserStateManager.IsUserLogged = true;
                UserStateManager.UserName = authParameters.Login;
                user = new UserModel() { Login = authParameters.Login };
                return RedirectToAction("Index");
            }
            else
                UserStateManager.IsUserLogged = false;
            return View();
        }
    }
}