using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Tests.Core.Settings
{
    public class TestSettings
    {
        /// <summary>Database address.</summary>
        public string DbAddress { get; set; }

        /// <summary>Database name.</summary>
        public string DbName { get; set; }

        /// <summary>Name of the default user to database access.</summary>
        public string DefaultUserName { get; set; }

        /// <summary>Password of the defaul user.</summary>
        public string DefaultUserPassword { get; set; }
    }
}
