using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragonfly.Models.Clients;
using Dragonfly.Core.Settings;
using Dragonfly.Database.MsSQL.LowLevel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

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
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentNullException(nameof(typeName));
            Client_Type newType = new Client_Type()
            {
                Type_Name = typeName
            };
            try
            {
                _Context.Client_Type.Add(newType);
                _Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                _Context.Client_Type.Remove(newType);
                List<ValidationError> validationErrors = new List<ValidationError>();
                foreach (var validResult in ex.EntityValidationErrors)
                {
                    validationErrors.AddRange(validResult.ValidationErrors.Select(
                        v => new ValidationError(v.PropertyName, v.ErrorMessage)));
                }
                throw new InsertDbDataException(validationErrors);
            }
            catch (DbUpdateException ex)
            {
                _Context.Client_Type.Remove(newType);
                throw new InsertDbDataException($"Update entity error: {ex.Message}");
            }
            return newType.ID_Client_Type;
        }

        public void CreateClient(ClientModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Name can not be empty", nameof(model));
            if (model.Type == null)
                throw new ArgumentException("Type can not be empty", nameof(model));
            Client client = model.ToClient();
            try
            {
                _Context.Client.Add(client);
                _Context.SaveChanges();
                model.ID = client.ID_Client;
            }
            catch (DbEntityValidationException ex)
            {
                _Context.Client.Remove(client);
                List<ValidationError> validationErrors = new List<ValidationError>();
                foreach (var validResult in ex.EntityValidationErrors)
                {
                    validationErrors.AddRange(validResult.ValidationErrors.Select(
                        v => new ValidationError(v.PropertyName, v.ErrorMessage)));
                }
                throw new InsertDbDataException(validationErrors);
            }
            catch (DbUpdateException ex)
            {
                _Context.Client.Remove(client);
                throw new InsertDbDataException($"Update entity error: {ex.Message}");
            }
        }

        /// <summary>
        /// Method retrieve all clients from the database.
        /// </summary>
        /// <returns>All clients from the DB.</returns>
        /// <exception cref="InvalidOperationException"/>
        public IEnumerable<ClientModel> GetAllClients()
        {
            try
            {
                var clients = (from t in _Context.Client
                               select t).ToList();
                return clients.Select(t => t.ToClientModel());
            }
            catch (Exception ex)
            {
                //TODO log
                throw new InvalidOperationException("Error occured on retrieving clients from DB.");
            }
        }

        public IEnumerable<ClientType> GetAvailableClientTypes()
        {
            try
            {
                var types = (from t in _Context.Client_Type
                             select t).ToList();
                return types.Select(t => t.ToClientType());
            }
            catch (Exception ex)
            {
                //TODO log
            }
            return new List<ClientType>();
        }
    }
}