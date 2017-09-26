using Dragonfly.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Providers
{
    /// <summary>
    /// A factory could generate all providers for a database access.
    /// </summary>
    public interface IDBFactory
    {
        /// <summary>
        /// Method generate a base DB provider.
        /// </summary>
        /// <returns>A base DB provider.</returns>
        IDataBaseProvider CreateDBProvider();

        /// <summary>
        /// Method generate a DB provider with the user acees operations.
        /// </summary>
        /// <returns>A user access provider.</returns>
        IUserAccessProvider CreateUserAccessProvider();

        IClientsProvider CreateClientsProvider();

        IProjectsProvider CreateProjectsProvider();
    }
}