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
        public EntitlementsController()
        {
        }

        // GET: Entitlements
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ControllersException]
        public ActionResult ClientEntitlements(decimal id, decimal projectId = 0)
        {//TODO check to ajax
            CheckUserAuthorization();

            if (id < 0)
                return RedirectToAction("Index", "Projects");
            EntitlementsModel model = new EntitlementsModel();
            try
            {
                List<EEntitlement> entitlements = null;
                if (projectId > 0)
                {
                    entitlements = EntitlementsProvider.GetEntitlements(id, projectId);
                    model.ProjectId = projectId;
                    model.ProjectName = ProjectsProvider.GetProject(projectId).ProjectName;
                }
                else
                    entitlements = EntitlementsProvider.GetEntitlementsForClient(id);

                AddEntitlementsToModel(id, model, entitlements);
            }
            catch (Exception ex)
            {//TODO handle
            }
            return View("Entitlements", model);
        }

        private static void AddEntitlementsToModel(decimal clientId, EntitlementsModel model, List<EEntitlement> entitlements)
        {
            List<ELicenseType> licTypes = EntitlementsProvider.GetLicenseTypes();
            List<EditEntitlementModel> entModels = new List<EditEntitlementModel>();
            foreach (EEntitlement dbEnt in entitlements)
            {
                var entModel = new EditEntitlementModel()
                {
                    LicenseTypeName = licTypes.FirstOrDefault(l => l.Id == dbEnt.LicenseTypeId).Name
                };
                entModels.Add(entModel.LoadEntitlement(dbEnt));
                entModel.Projectname = ProjectsProvider.GetProject(entModel.ProjectId).ProjectName;
            }
            model.Entitlemens = entModels;
            model.ClientId = clientId;
            model.ClientInternalName = ClientsProvider.GetClient(clientId).Name;
        }
    }
}