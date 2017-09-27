using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Entities
{
    /// <summary>Class represent a type of an entitlement.</summary>
    public class ELicenseType
    {
        public decimal Id { get; set; }

        public string Name { get; set; }

        public DateTime DateCreated { get; set; }
    }
}