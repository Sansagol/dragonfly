using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Dragonfly.Core.Settings
{
    public class StubSettingsReader : ISettingsReader
    {
        public DatabaseAccessConfiguration GetDbAccessSettings()
        => new DatabaseAccessConfiguration()
            {
                DbName = "Dragonfly",
                ServerName = "127.0.1.1",
                UserName = "UserName",
                Password = "SomeUnbreakablePassword"
        };

        public string GetLogDirectory()
        {
            return Path.Combine(Path.GetTempPath(), "DragonflyTestLogs");
        }
    }
}