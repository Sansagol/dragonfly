using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dragonfly.Models.Clients
{
    /// <summary>
    /// Class represent a custom client.
    /// </summary>
    public class ClientModel
    {
        /// <summary>ID of the client.</summary>
        public decimal ID { get; set; }

        /// <summary>
        /// Official name of the client.
        /// </summary>
        [Required(ErrorMessage = "Name can not be null")]
        public string Name { get; set; }

        /// <summary>
        /// Alternative name of the client.
        /// </summary>
        public string InnerName { get; set; }

        public string INN { get; set; }
        public string OGRN { get; set; }
        public string KPP { get; set; }

        /// <summary>
        /// Type of client
        /// </summary>
        [Required(ErrorMessage = "Pleaase select a client type")]
        public ClientType Type { get; set; }
    }
}