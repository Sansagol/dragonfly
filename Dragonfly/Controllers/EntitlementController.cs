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
using Dragonfly.Database.Providers;
using Dragonfly.Database.Entities;

namespace Dragonfly.Controllers
{
    public class EntitlementController : Controller
    {
        private IUserAuthenticateStateManager _UserStateManager = null;
        private IEntitlementsProvider _EntitlementsProvider;
        private IClientsProvider _ClientsProvider;

        public EntitlementController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
            _EntitlementsProvider = BaseBindings.DBFactory.CreateEntitlementsProvider();
            _ClientsProvider = BaseBindings.DBFactory.CreateClientsProvider();
        }

        [HttpGet]
        [ControllersException]
        public ActionResult NewEntitlement(decimal projectId)
        {
            ViewBag.Logged = _UserStateManager.CheckUserAccess(Request, Response);
            try
            {
                EditEntitlementModel model = new EditEntitlementModel(projectId, 0);
                LoadLicenseTypes(model);
                return View("EditEntitlement", model);
            }
            catch (Exception ex)
            {//TODO test it
                ViewBag.Error = "Error on creation entitlement";
                return new EntitlementsController().Index();
            }
        }

        private void LoadLicenseTypes(EditEntitlementModel model)
        {
            var availableLicanseTypes = _EntitlementsProvider.GetLicenseTypes();
            model.AvailableLicanseTypes = availableLicanseTypes.Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() });
        }
        private void LoadClients()
        {
            var availableClients = _ClientsProvider.GetClients();
        }

        [HttpPost]
        [ControllersException]
        public ActionResult SaveEntitlement(EditEntitlementModel model)
        {
            if (model.DateBegin > model.DateEnd)
                ModelState.AddModelError("DateEnd", "The end date must be greather then begin date");
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Entitlement", model.EntitlementId);
            }
            LoadLicenseTypes(model);
            return View("EditEntitlement", model);
        }
    }
}