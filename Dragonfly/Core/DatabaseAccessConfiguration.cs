using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Core
{
    /// <summary>Class represent the parameters fo DB access.</summary>
    public class DatabaseAccessConfiguration
    {
        /// <summary>Sever name, IP.</summary>
        public string ServerName { get; set; }
        /// <summary>Port to access database.</summary>
        public int Port { get; set; }
        /// <summary>Database name.</summary>
        public string DbName { get; set; }
        /// <summary>Name of user to access to database.</summary>
        public string UserName { get; set; }
        /// <summary>Password to access.</summary>
        public string Password { get; set; }
    }
}