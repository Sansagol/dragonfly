using Dragonfly.Core;
using Dragonfly.Database.Providers;
using Dragonfly.Models.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    public class ClientController : Controller
    {
        private IUserStateManager _UserStateManager = null;
        private ICookiesManager _CookManager = null;
        private IDBFactory _DatabaseFactory = null;

        public ClientController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
            _CookManager = BaseBindings.CookiesManager;
            _DatabaseFactory = BaseBindings.DBFactory;
        }

        /// <summary>
        /// Method run creation a new client.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Add()
        {
            if (_UserStateManager.CheckUserAccess(Request))
            {
                ViewBag.Logged = true;
                var model = new CreateClientModel();
                


                return View(new CreateClientModel());
            }
            else
            {
                ViewBag.Logged = false;
                return View();
            }
        }

        /// <summary>
        /// Method check parameters and create a new client.
        /// </summary>
        /// <param name="model">Model for creation a client.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CreateClientModel model)
        {
            if (_UserStateManager.CheckUserAccess(Request))
            {
                return RedirectToAction("Index", "Clients");
            }
            else
            {
                return RedirectToAction("Index", "Main");
            }
        }
    }
}