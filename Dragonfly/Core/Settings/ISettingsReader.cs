using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Core.Settings
{
    interface ISettingsReader
    {
        /// <summary>Get the log directory from a config file.</summary>
        string GetLogDirectory();

        /// <summary>Method create a settings to database access.</summary>
        /// <returns>Settings to access to database.</returns>
        DatabaseAccessConfiguration GetDbAccessSettings();
    }
}
