using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.Clients
{
    /// <summary>
    /// Represent a type of client
    /// </summary>
    public class ClientType
    {
        public decimal ID { get; set; }

        /// <summary>
        /// Name of the type (Organization, human...).
        /// </summary>
        public string TypeName { get; set; }
    }
}