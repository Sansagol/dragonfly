using Dragonfly.Core;
using Dragonfly.Database.Entities;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.Entitlement
{
    public class EditEntitlementModel
    {
        private IClientsProvider _ClientsProvider = null;
        private IProjectsProvider _ProjectsProvider = null;

        public EEntitlement EntitlementDetails { get; set; }

        public List<EClient> AvailableClients { get; private set; }

        public EditEntitlementModel()
        {
        }

        public EditEntitlementModel(decimal projectId, decimal entitlementId)
        {
            if (projectId < 0)
                throw new ArgumentException("the project id must be greather than 0");

            _ProjectsProvider = BaseBindings.DBFactory.CreateProjectsProvider();
            _ClientsProvider = BaseBindings.DBFactory.CreateClientsProvider();

            LoadClients();

            if (entitlementId > 0)
                LoadEntitlement(entitlementId);
            else
            {
                EntitlementDetails = new EEntitlement()
                {
                };
                EntitlementDetails.Project = _ProjectsProvider.GetProject(projectId);
            }
        }

        private void LoadClients()
        {
            AvailableClients = new List<EClient>();
            AvailableClients.AddRange(_ClientsProvider.GetClients());
        }

        private void LoadEntitlement(decimal entitlementId)
        {
            EntitlementDetails = _ClientsProvider.GetEntitlement(entitlementId);
        }

        public void SaveEntitlement()
        {
        }
    }
}