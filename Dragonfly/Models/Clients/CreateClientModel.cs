using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragonfly.Models.Clients
{
    public class CreateClientModel
    {
        /// <summary>
        /// Current client.
        /// </summary>
        public ClientModel Client { get; set; }

        /// <summary>
        /// Available types of the clients.
        /// </summary>
        public SelectListItem[] AvailableTypes { get; set; }
    }
}