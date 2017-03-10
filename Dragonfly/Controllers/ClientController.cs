using Dragonfly.Core;
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

        public ClientController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
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
                return View(new ClientModel());
            }
            else
            {
                ViewBag.Logged = false;
                return View();
            }
        }
    }
}