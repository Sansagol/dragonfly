using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Entities
{
    public class EClient
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string INN { get; set; }
        public string OGRN { get; set; }
        public string KPP { get; set; }
        public string InnerName { get; set; }

        //TODO add address
        //TODO add client type
        //TODO add email
        //TODO add phone
    }
}