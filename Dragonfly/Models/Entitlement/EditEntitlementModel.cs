using Dragonfly.Binders;
using Dragonfly.Core;
using Dragonfly.Database.Entities;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Models.Entitlement
{
    //[ModelBinder(typeof(EntitlementModelBinder))]
    public class EditEntitlementModel
    {
        private IClientsProvider _ClientsProvider = null;
        private IEntitlementsProvider _EntitlementsProvider;
        private IProjectsProvider _ProjectsProvider = null;

        //public EEntitlement EntitlementDetails { get; set; }

        public int EntitlementId { get; set; }

        /// <summary>Date of entitlement starts</summary>
        [Required(ErrorMessage = "Invalid date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DateBegin { get; set; }
        /// <summary>End date of the entitlement</summary>
        [Required(ErrorMessage = "Invalid date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DateEnd { get; set; }

        /// <summary>Count of the sold licenses</summary>
        [Range(1, 100, ErrorMessage = "Please enter a licenses count for this entitlement")]
        public decimal LicensesCount { get; set; }
        /// <summary>Some description.</summary>
        public string Details { get; set; }

        /// <summary>Type of the sold license.</summary>
        [Required(ErrorMessage = "Please select a license type for this entitlement")]
        public decimal LicTypeId { get; set; }
        public IEnumerable<SelectListItem> AvailableLicanseTypes { get; set; }

        /// <summary>Get the project for this entitlement.</summary>
        [Required(ErrorMessage = "Please select a project for this entitlement")]
        public decimal ProjectId { get; set; }
        public string Projectname { get; set; }

        /// <summary>Client which bougcht the product.</summary>
        [Required(ErrorMessage = "Please select a client for this entitlement")]
        public decimal ClientId { get; set; }

        public IEnumerable<SelectListItem> AvailableClients { get; set; }

        public EditEntitlementModel()
        {
            DateBegin = DateTime.Now.Date;
            DateEnd = DateBegin.AddDays(1).Date;

        }

        public void LoadEntitlement(EEntitlement entitlement)
        {
            DateBegin = entitlement.DateBegin;
            DateEnd = entitlement.DateEnd;
            LicensesCount = entitlement.LicensesCount;
            Details = entitlement.Details;
            LicTypeId = entitlement.LicType.Id;
            ClientId = entitlement.Client.Id;
        }
    }
}