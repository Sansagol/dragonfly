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
using Dragonfly.Core;

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

        public decimal CreateAClientType(string typeName, decimal id)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentNullException(nameof(typeName));
            Client_Type newType = new Client_Type()
            {
                Type_Name = typeName,
                ID_Client_Type = id
            };
            using (var context = _ContextGenerator.GenerateContext())
            {
                try
                {
                    context.Client_Type.Add(newType);
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    context.Client_Type.Remove(newType);
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
                    context.Client_Type.Remove(newType);
                    throw new InsertDbDataException($"Update entity error: {ex.Message}");
                }
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
            using (var context = _ContextGenerator.GenerateContext())
            {
                try
                {
                    context.Client.Add(client);
                    context.SaveChanges();
                    model.ID = client.ID_Client;
                }
                catch (DbEntityValidationException ex)
                {
                    context.Client.Remove(client);
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
                    context.Client.Remove(client);
                    throw new InsertDbDataException($"Update entity error: {ex.GetFullMessage()}");
                }
            }
        }

        /// <summary>
        /// Method retrieve all clients from the database.
        /// </summary>
        /// <returns>All clients from the DB.</returns>
        /// <exception cref="InvalidOperationException"/>
        public List<ClientModel> GetAllClients()
        {
            try
            {
                using (var context = _ContextGenerator.GenerateContext())
                {
                    var clients = (from t in context.Client
                                   select t).ToList();
                    return clients.Select(t => t.ToClientModel()).ToList();
                }
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
                using (var context = _ContextGenerator.GenerateContext())
                {
                    var types = (from t in context.Client_Type
                                 select t).ToList();
                    return types.Select(t => t.ToClientType());
                }
            }
            catch (Exception ex)
            {
                //TODO log
            }
            return new List<ClientType>();
        }
    }
}