using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Core.Logging
{
    /// <summary>
    /// Interface must implements a log configurators.
    /// </summary>
    interface ILogConfiguration
    {
        /// <summary>Method initialize a log parameters.</summary>
        void InitConfig();
    }
}
