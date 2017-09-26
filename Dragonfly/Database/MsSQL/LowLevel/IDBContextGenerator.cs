using Dragonfly.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Database.MsSQL.LowLevel
{
    /// <summary>
    /// Interface realize classes, which generate the db contexts.
    /// </summary>
    interface IDBContextGenerator
    {
        DragonflyEntities GenerateContext();
    }
}
