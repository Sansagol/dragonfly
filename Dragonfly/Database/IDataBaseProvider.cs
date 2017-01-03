using Dragonfly.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.Database
{
    /// <summary>
    /// Interface implements providers, which will be
    /// provide access to databases.
    /// </summary>
    interface IDataBaseProvider
    {
        /// <summary>The method initialize connection with database.</summary>
        ///<param name="accessConfigurations">Parameters to DB access.</param>
        /// <returns>Created context. null if fail.</returns>
        DbContext GetNewContext(DatabaseAccessConfiguration accessConfigurations);
    }
}
