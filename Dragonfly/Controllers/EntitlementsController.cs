using Dragonfly.Core;
using Dragonfly.Core.UserAccess;
using Dragonfly.Database.Entities;
using Dragonfly.Models.Entitlement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Controllers
{
    public class EntitlementsController : BaseController
    {
        private IUserAuthenticateStateManager _UserStateManager = null;

        public EntitlementsController()
        {
            _UserStateManager = BaseBindings.UsrStateManager;
        }

        // GET: Entitlements
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ControllersException]
        public ActionResult ClientEntitlements(decimal clientId, decimal projectId)
        {//TODO check to ajax
            _UserStateManager.CheckUserAccess(Request, Response);
            ViewBag.Logged = true;
            ViewBag.UserName = BaseBindings.CookiesManager.GetCookieValue(
                Request,
                CookieType.UserName);

            if (clientId < 0 || projectId < 0)
                return RedirectToAction("Index", "Projects");
            EntitlementsModel model = new EntitlementsModel();
            try
            {
                var entsProvider = BaseBindings.DBFactory.CreateEntitlementsProvider();
                List<EEntitlement> entitlements = entsProvider.GetEntitlements(clientId, projectId);
                List<ELicenseType> licTypes = entsProvider.GetLicenseTypes();

                List<EditEntitlementModel> entModels = new List<EditEntitlementModel>();
                foreach (EEntitlement dbEnt in entitlements)
                {
                    var entModel = new EditEntitlementModel()
                    {
                        LicenseTypeName = licTypes.FirstOrDefault(l => l.Id == dbEnt.LicenseTypeId).Name
                    };
                    entModels.Add(entModel.LoadEntitlement(dbEnt));

                }
                model.Entitlemens = entModels;
                model.ClientId = clientId;
                model.ClientInternalName = ClientsProvider.GetClient(clientId).Name;
                model.ProjectId = projectId;
                model.ProjectName = BaseBindings.DBFactory.CreateProjectsProvider().GetProject(projectId).ProjectName;
            }
            catch (Exception ex)
            {//TODO handle
            }
            return View("Entitlements", model);
        }
    }
}