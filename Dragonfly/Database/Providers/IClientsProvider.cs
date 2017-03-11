using Dragonfly.Core.Settings;
using Dragonfly.Models.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Providers
{
    /// <summary>
    /// Interface represent methods to manipulate of clients.
    /// </summary>
    public interface IClientsProvider : IDisposable
    {
        void Initialize(DatabaseAccessConfiguration accessConfigurations);

        void CreateClient(ClientModel model);

        /// <summary>
        /// Method retrieve and return all available types of clients.
        /// </summary>
        /// <returns></returns>
        ClientType GetAvailableClientTypes();
    }
}