using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.Entitlement
{
    public class EntitlementsModel
    {
        public List<EditEntitlementModel> Entitlemens { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalEntitlements { get; set; }

        public decimal ClientId { get; set; }
        public string ClientInternalName { get; set; }
        public decimal ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}