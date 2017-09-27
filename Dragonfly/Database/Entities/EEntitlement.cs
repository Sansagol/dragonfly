using System;
using System.Collections.Generic;
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
        public DateTime DateBegin { get; set; }
        /// <summary>End date of the entitlement</summary>
        public DateTime DateEnd { get; set; }
        /// <summary>Count of the sold licenses</summary>
        public decimal LicensesCount { get; set; }
        /// <summary>Some description.</summary>
        public string Details { get; set; }
        /// <summary>Type of the sold license.</summary>
        public ELicenseType LicType { get; set; }
        /// <summary>Get the project for this entitlement.</summary>
        public EProject Project { get; set; }
        /// <summary>Client which bougcht the product.</summary>
        public EClient Client { get; set; }
    }
}