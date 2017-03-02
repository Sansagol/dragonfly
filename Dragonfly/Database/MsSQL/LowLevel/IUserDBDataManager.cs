using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Database.MsSQL.LowLevel
{
    /// <summary>
    /// Represent a user operations with DB data.
    /// </summary>
    interface IUserDBDataManager: IDBDataManager
    {
        /// <summary>Method retrieve a user by it id.</summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        User GetUserById(decimal userId);
    }
}
