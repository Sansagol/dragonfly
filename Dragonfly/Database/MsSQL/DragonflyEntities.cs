using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL
{
    public partial class DragonflyEntities
    {
        public DragonflyEntities(string connectionString) :
            base(connectionString)
        {
        }
    }
}