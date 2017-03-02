using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Database.MsSQL.LowLevel
{
    interface IDBDataManager
    {
        void Initialize(DbContext context);
    }
}
