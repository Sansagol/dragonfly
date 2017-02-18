using Dragonfly.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Tests
{
    internal class Common
    {
        public static readonly DatabaseAccessConfiguration Connectionconfig =
            new DatabaseAccessConfiguration()
        {
            DbName = "Dragonfly.Test",
            ServerName = "10.10.0.117",
            UserName = "Unit_Tester",
            Password = "SelectAllPasswords"
        };
    }
}
