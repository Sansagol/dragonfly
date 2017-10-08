using Dragonfly.Core;
using Dragonfly.Core.UserAccess;
using Dragonfly.Models.Entitlement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dragonfly.Controllers
{
    public class EntitlementController : Controller
    {
        private IUserAuthenticateStateManager _UserStateManager = null;

        public EntitlementController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
        }

        [HttpGet]
        [ControllersException]
        public ActionResult NewEntitlement(decimal projectId)
        {
            ViewBag.Logged = _UserStateManager.CheckUserAccess(Request, Response);
            try
            {
                EditEntitlementModel model = new EditEntitlementModel(projectId, 0);
                return View("EditEntitlement", model);
            }
            catch (Exception ex)
            {//TODO test it
                ViewBag.Error = "Error on creation entitlement";
                return new EntitlementsController().Index();
            }
        }

        [HttpPost]
        [ControllersException]
        public ActionResult SaveEntitlement(EditEntitlementModel model)
        {
            return View();
        }
    }
}