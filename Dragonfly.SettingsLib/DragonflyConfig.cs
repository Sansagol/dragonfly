using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragonfly.SettingsLib
{
    /// <summary>Base class of settings, which will be save in a file.</summary>
    public class DragonflyConfig
    {
        /// <summary>Configuration of database access.</summary>
        public DatabaseConfig DbConfiguration { get; set; }

        /// <summary>Initialize all properties.</summary>
        public DragonflyConfig()
        {
            DbConfiguration = new DatabaseConfig();
        }
    }
}
