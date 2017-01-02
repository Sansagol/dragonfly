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
    interface IDataBaseProvider: IDisposable
    {
        /// <summary>Context of database.</summary>
        DbContext Context { get; }

        /// <summary>The method initialize connection with database.</summary>
        ///<param name="accessConfigurations">Parameters to DB access.</param>
        /// <returns>Connection status.</returns>
        bool InitializeConnection(DatabaseAccessConfiguration accessConfigurations);
    }
}
