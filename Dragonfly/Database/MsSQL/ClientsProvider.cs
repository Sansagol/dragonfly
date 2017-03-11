using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragonfly.Models.Clients;
using Dragonfly.Core.Settings;

namespace Dragonfly.Database.MsSQL
{
    public class ClientsProvider : IClientsProvider
    {
        public void Initialize(DatabaseAccessConfiguration accessConfigurations)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
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