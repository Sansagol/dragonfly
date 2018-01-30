using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    public class EntitlementsController : Controller
    {
        // GET: Entitlements
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ClientEntitlements(decimal clientId, decimal projectId)
        {//TODO check to ajax
            if (clientId < 0 || projectId < 0)
                return RedirectToAction("Index", "Projects");
            
            return View();
        }
    }
}