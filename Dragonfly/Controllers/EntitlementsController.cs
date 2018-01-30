using Dragonfly.Core;
using Dragonfly.Database.Entities;
using Dragonfly.Models.Entitlement;
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
            EntitlementsModel model = new EntitlementsModel();
            try
            {
                List<EEntitlement> entitlements = BaseBindings.DBFactory.CreateEntitlementsProvider()
                    .GetEntitlements(clientId, projectId);
                List<EditEntitlementModel> entModels = new List<EditEntitlementModel>();
                foreach (EEntitlement dbEnt in entitlements)
                {
                    entModels.Add(new EditEntitlementModel().LoadEntitlement(dbEnt));
                }
                model.Entitlemens = entModels;
                model.ClientId = clientId;
                model.ProjectId = projectId;
            }
            catch (Exception ex)
            {//TODO handle
            }
            return View("Entitlements", model);
        }
    }
}