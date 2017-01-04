using Dragonfly.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Core
{
    internal class BaseBindings
    {
        public static ISettingsReader Settingsreader { get; }

        static BaseBindings()
        {
            Settingsreader = new StubSettingsReader();
        }
    }
}