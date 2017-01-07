using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.SettingsManager.DataSources
{
    public partial class DragonflyEntities
    {
        public DragonflyEntities(string connectionString) :
            base(connectionString)
        {
        }
    }
}