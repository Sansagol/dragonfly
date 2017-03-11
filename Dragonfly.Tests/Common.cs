using Dragonfly.Core.Settings;
using System;
using System.IO;
using System.Linq;

namespace Dragonfly.Tests
{
    internal class Common
    {        /// <summary>Test config file.</summary>
        private static readonly string _DefaultPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Dragonfly",
            "config.test.xml");

        public static readonly DatabaseAccessConfiguration Connectionconfig = null;

        static Common()
        {
            Connectionconfig = new SettingsLibReader().GetDbAccessSettings(_DefaultPath);
        }
    }
}
