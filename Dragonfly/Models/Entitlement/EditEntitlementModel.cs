using Dragonfly.Core;
using Dragonfly.Database.Entities;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.Entitlement
{
    public class EditEntitlementModel
    {
        private IClientsProvider _ClientsProvider = null;
        private IProjectsProvider _ProjectsProvider = null;

        public EEntitlement EntitlementDetails { get; set; }

        /// <summary>Date of entitlement starts</summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateBegin { get; set; }
        /// <summary>End date of the entitlement</summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateEnd { get; set; }

        /// <summary>Count of the sold licenses</summary>
        [Range(1, 100, ErrorMessage = "Please enter a licenses count for this entitlement")]
        public decimal LicensesCount { get; set; }
        /// <summary>Some description.</summary>
        public string Details { get; set; }

        /// <summary>Type of the sold license.</summary>
        [Required(ErrorMessage = "Please select a license type for this entitlement")]
        public ELicenseType LicType { get; set; }

        /// <summary>Get the project for this entitlement.</summary>
        [Required(ErrorMessage = "Please select a project for this entitlement")]
        public EProject Project { get; set; }

        /// <summary>Client which bougcht the product.</summary>
        [Required(ErrorMessage = "Please select a client for this entitlement")]
        public int ClientId { get; set; }

        public List<EClient> AvailableClients { get; private set; }

        public EditEntitlementModel()
        {
            DateEnd = DateBegin = DateTime.Now.Date;
        }

        public EditEntitlementModel(decimal projectId, decimal entitlementId):
            this()
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