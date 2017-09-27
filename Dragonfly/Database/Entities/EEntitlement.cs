using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Entities
{
    public class EEntitlement
    {
        public decimal Id { get; set; }

        /// <summary>When this entitlement was create.</summary>
        public DateTime DateCreated { get; set; }
        /// <summary>A user which created this entitlement.</summary>
        public EUser Creator { get; set; }

        /// <summary>Date of entitlement starts</summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateBegin { get; set; }
        /// <summary>End date of the entitlement</summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateEnd { get; set; }
        
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
        public EClient Client { get; set; }

        public EEntitlement()
        {
            DateBegin = DateTime.Now.Date;
            DateEnd = DateTime.Now.Date.AddMonths(1);
        }
    }
}