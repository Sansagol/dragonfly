using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragonfly.Models.Clients;
using Dragonfly.Core.Settings;
using Dragonfly.Database.MsSQL.LowLevel;
using System.Data.Entity;

namespace Dragonfly.Database.MsSQL
{
    /// <summary>
    /// Provider manipulate the data of clients.
    /// </summary>
    class ClientsProvider : DataProvider, IClientsProvider
    {
        public ClientsProvider(IDBContextGenerator contextgenerator) :
            base(contextgenerator)
        {
        }

        public decimal CreateAClientType(string typeName)
        {
            throw new NotImplementedException();
        }

        public void CreateClient(ClientModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Name can not be empty", nameof(model));
            if (model.Type == null)
                throw new ArgumentException("Type can not be empty", nameof(model));

        }

        public IEnumerable<ClientType> GetAvailableClientTypes()
        {
            throw new NotImplementedException();
        }
    }
}