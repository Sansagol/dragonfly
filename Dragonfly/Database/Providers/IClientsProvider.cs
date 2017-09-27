using Dragonfly.Core.Settings;
using Dragonfly.Database.Entities;
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
    public interface IClientsProvider : IDataProvider
    {
        void CreateClient(ClientModel model);

        /// <summary>
        /// Method creates a type of client in database.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="id">Id of the new type (is not auto increment).</param>
        /// <returns>Created type id.</returns>
        decimal CreateAClientType(string typeName, decimal id);

        /// <summary>
        /// Method retrieve and return all available types of clients.
        /// </summary>
        /// <returns>All availave client types.</returns>
        IEnumerable<ClientType> GetAvailableClientTypes();

        /// <summary>
        /// Method fetch all flients from the database.
        /// </summary>
        /// <returns>All existing clients.</returns>
        List<ClientModel> GetAllClients();

        /// <summary>Method fetch all entitlements for the current project.</summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        List<EEntitlement> GetEntitlementsForProject(decimal projectId);
        /// <summary>Method fetch entitlement for the project.</summary>
        /// <param name="projectId"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<EEntitlement> GetEntitlementsForProject(decimal projectId, int offset, int count);
    }
}