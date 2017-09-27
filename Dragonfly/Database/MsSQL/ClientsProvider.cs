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
using Dragonfly.Database.Entities;
using Dragonfly.Database.MsSQL.Converters;

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

        public List<EEntitlement> GetEntitlementsForProject(decimal projectId)
        {
            if (projectId < 1)
                throw new ArgumentException("The project id must be greather than 0.");
            List<EEntitlement> entitlements = new List<EEntitlement>();

            using (var context = _ContextGenerator.GenerateContext())
            {
                var dbEntitlements = (from e in context.Product_License
                                      where e.ID_Project == projectId
                                      select e).ToList();
                if (dbEntitlements?.Count > 0)
                {
                    var project = (from p in context.Project
                                   where p.ID_Project == projectId
                                   select p).First().ToEProject();
                    IEnumerable<EClient> clients = LoadClients(dbEntitlements);
                    IEnumerable<EUser> users = LoadCreators(dbEntitlements);
                    IEnumerable<ELicenseType> licenseTypes = LoadLicenseTypes(dbEntitlements);

                    foreach (var dbEntitlement in dbEntitlements)
                    {
                        EEntitlement entitlement = dbEntitlement.ToEEntitlement();
                        entitlement.Client = clients.FirstOrDefault(c => c.Id == dbEntitlement.ID_Client);
                        entitlement.Creator = users.FirstOrDefault(c => c.Id == dbEntitlement.ID_User_Creator);
                        entitlement.LicType = licenseTypes
                            .FirstOrDefault(c => c.Id == dbEntitlement.ID_Product_License);
                        entitlement.Project = project;
                        entitlements.Add(entitlement);
                    }
                }
            }
            return entitlements;
        }

        #region Load the client relation data
        private IEnumerable<EClient> LoadClients(List<Product_License> dbEntitlements)
        {
            List<decimal> clientsIds = dbEntitlements.Select(e => e.ID_Client).Distinct().ToList();
            List<EClient> clients = new List<EClient>();
            using (var context = _ContextGenerator.GenerateContext())
            {
                foreach (var dbEnt in dbEntitlements)
                {
                    var client = (from l in context.Client
                                  where l.ID_Client == dbEnt.ID_Client
                                  select l).First().ToEClient();
                    if (client != null)
                        clients.Add(client);
                }
            }
            return clients;
        }

        private IEnumerable<EUser> LoadCreators(List<Product_License> dbEntitlements)
        {
            List<decimal> usersIds = dbEntitlements.Select(e => e.ID_User_Creator).Distinct().ToList();
            List<EUser> users = new List<EUser>();
            using (var context = _ContextGenerator.GenerateContext())
            {
                foreach (var dbEnt in dbEntitlements)
                {
                    var user = (from l in context.User
                                where l.ID_User == dbEnt.ID_User_Creator
                                select l).First().ToEUser();
                    if (user != null)
                        users.Add(user);
                }
            }
            return users;
        }

        private IEnumerable<ELicenseType> LoadLicenseTypes(List<Product_License> dbEntitlements)
        {
            List<decimal> productLicenses = dbEntitlements
                .Select(e => e.ID_Product_License)
                .Distinct()
                .ToList();
            using (var context = _ContextGenerator.GenerateContext())
            {
                List<ELicenseType> licenseTypes = new List<ELicenseType>();
                foreach (var dbEnt in dbEntitlements)
                {
                    var licenseType = (from l in context.License_Type
                                       where l.ID_License_Type == dbEnt.ID_License_Type
                                       select l).First().ToELicenseType();
                    if (licenseType != null)
                        licenseTypes.Add(licenseType);
                }
                return licenseTypes;
            }
        }
        #endregion

        public List<EEntitlement> GetEntitlementsForProject(decimal projectId, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}