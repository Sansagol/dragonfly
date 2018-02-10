﻿using Dragonfly.Core;
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
        private IProjectsProvider _ProjectsProvider;

        public EntitlementController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
            _EntitlementsProvider = BaseBindings.DBFactory.CreateEntitlementsProvider();
            _ClientsProvider = BaseBindings.DBFactory.CreateClientsProvider();
            _ProjectsProvider = BaseBindings.DBFactory.CreateProjectsProvider();
        }

        [HttpGet]
        [ControllersException]
        public ActionResult EditEntitlement(decimal entitlementId)
        {
            ViewBag.Logged = _UserStateManager.CheckUserAccess(Request, Response);
            if (entitlementId < 1)
                RedirectToAction("Index", "Entitlements");
            decimal currentUser = _UserStateManager.GetUserIdFromCookies(Request);
            var entitlement = _EntitlementsProvider.GetEntitlement(entitlementId);
            if (entitlement == null)
                RedirectToAction("Index", "Entitlements");
            if (entitlement.UserCreatorId == currentUser)//TODO check by project members
            {
                EditEntitlementModel model = new EditEntitlementModel();
                model.LoadEntitlement(entitlement);
                LoadThirdElementsData(model);
                return View("EditEntitlement", model);
            }
            return RedirectToAction("Index", "Entitlements");
        }

        private void LoadThirdElementsData(EditEntitlementModel model)
        {
            LoadLicenseTypes(model);
            LoadClients(model);
            LoadProject(model);
        }

        [HttpGet]
        [ControllersException]
        public ActionResult NewEntitlement(decimal projectId)
        {
            ViewBag.Logged = _UserStateManager.CheckUserAccess(Request, Response);
            try
            {
                EditEntitlementModel model = new EditEntitlementModel();
                model.ProjectId = projectId;
                LoadThirdElementsData(model);

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
        private void LoadClients(EditEntitlementModel model)
        {
            var availableClients = _ClientsProvider.GetClients();
            model.AvailableClients = availableClients.Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() });
        }

        private void LoadProject(EditEntitlementModel model)
        {
            var project = _ProjectsProvider.GetProject(model.ProjectId);
            model.Projectname = project.ProjectName;
        }

        [HttpPost]
        [ControllersException]
        public ActionResult SaveEntitlement(EditEntitlementModel model)
        {
            if (model.DateBegin > model.DateEnd)
                ModelState.AddModelError("DateEnd", "The end date must be greather then begin date");
            if (ModelState.IsValid)
            {
                _UserStateManager.CheckUserAccess(Request, Response);//Check this user before save
                EEntitlement ent = model.ToEEntitlement();
                _EntitlementsProvider.SaveEntitlement(ent, _UserStateManager.GetUserIdFromCookies(Request));
                model.EntitlementId = ent.Id;
                return RedirectToAction("EditEntitlement", "Entitlement", new { entitlementId = model.EntitlementId });
            }
            LoadThirdElementsData(model);
            return View("EditEntitlement", model);
        }
    }
}