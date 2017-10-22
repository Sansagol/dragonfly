using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragonfly.SettingsLib
{
    /// <summary>Configuration of database.</summary>
    public class DatabaseConfig
    {
        /// <summary>Database address.</summary>
        public string DbAddress { get; set; } = "127.0.0.1";

        /// <summary>Database name.</summary>
        public string DbName { get; set; } = "Dragonfly";

        /// <summary>Name of the default user to database access.</summary>
        public string DefaultUserName { get; set; } = "User";

        /// <summary>Password of the defaul user.</summary>
        public string DefaultUserPassword { get; set; } = "Passw0rd";
    }
}
