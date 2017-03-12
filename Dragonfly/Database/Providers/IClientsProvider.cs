using Dragonfly.Core.Settings;
using Dragonfly.Models.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Providers
{
    /// <summary>
    /// Interface represent methods to manipulate with clients objects.
    /// </summary>
    public interface IClientsProvider : IDataProvider, IDisposable
    {
        void CreateClient(ClientModel model);

        /// <summary>
        /// Method creates a type of client in database.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>Created type id.</returns>
        decimal CreateAClientType(string typeName);

        /// <summary>
        /// Method retrieve and return all available types of clients.
        /// </summary>
        /// <returns>All availave client types.</returns>
        IEnumerable<ClientType> GetAvailableClientTypes();

        /// <summary>
        /// Method fetch all flients from the database.
        /// </summary>
        /// <returns>All existing clients.</returns>
        IEnumerable<ClientModel> GetAllClients();
    }
}