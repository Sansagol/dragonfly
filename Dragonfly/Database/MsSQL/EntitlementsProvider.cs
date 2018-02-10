using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragonfly.Database.Entities;
using Dragonfly.Database.MsSQL.LowLevel;
using Dragonfly.Database.MsSQL.Converters;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Migrations;

namespace Dragonfly.Database.MsSQL
{
    class EntitlementsProvider : DataProvider, IEntitlementsProvider
    {

        public EntitlementsProvider(IDBContextGenerator contextGenerator) :
            base(contextGenerator)
        {
        }

        public EEntitlement GetEntitlement(decimal entitlementId)
        {
            try
            {
                using (var context = _ContextGenerator.GenerateContext())
                {
                    var entitlement = (from ent in context.Product_License
                                       where ent.ID_Product_License == entitlementId
                                       select ent).FirstOrDefault();
                    return entitlement.ToEEntitlement();
                }
            }
            catch (Exception ex)
            {//TODO to log
                throw new InvalidOperationException("Error occured on retrieving the entitlement from DB.");
            }
        }

        public List<ELicenseType> GetLicenseTypes()
        {
            try
            {
                using (var context = _ContextGenerator.GenerateContext())
                {
                    var types = (from lt in context.License_Type
                                 select lt).ToArray();
                    return types.Select(t => t.ToELicenseType()).ToList();
                }
            }
            catch (Exception ex)
            {//TODO to log
                throw new InvalidOperationException("Error occured on retrieving the license types from DB.");
            }
        }

        /// <summary>Method save the entitlement in the database.</summary>
        /// <param name="entitlementToSave"></param>
        /// <param name="ownerId">The user which created the entity.</param>
        /// <returns></returns>
        public bool SaveEntitlement(EEntitlement entitlementToSave, decimal ownerId)
        {
            if (entitlementToSave == null)
                throw new ArgumentNullException(nameof(entitlementToSave));
            if (ownerId < 1)
                throw new ArgumentNullException(nameof(ownerId), "Creator_id must be greather than 0");

            using (var context = _ContextGenerator.GenerateContext())
            {
                var license = entitlementToSave.ToProductLicense();
                license.ID_User_Creator = ownerId;
                Product_License dbLicense =
                    context.Product_License.FirstOrDefault(l => l.ID_Product_License == license.ID_Product_License);
                {
                    if (dbLicense != null)
                    {
                        license.Date_Created = dbLicense.Date_Created;
                        context.Product_License.AddOrUpdate(license);
                    }
                    else
                    {
                        context.Product_License.Add(license);
                    }
                    context.SaveChanges();
                    entitlementToSave.Id = license.ID_Product_License;
                }
            }
            return true;
        }

        public List<EEntitlement> GetEntitlements(decimal clientId, decimal projectId)
        {
            if (clientId < 1)
                throw new ArgumentNullException(nameof(clientId), "Id of the client must be greather than 0");
            if (projectId < 1)
                throw new ArgumentNullException(nameof(projectId), "Id of the project must be greather than 0");

            List<EEntitlement> entitlements = new List<EEntitlement>();
            using (var context = _ContextGenerator.GenerateContext())
            {
                var dbEntitlements = (from e in context.Product_License
                                      where e.ID_Client == clientId &&
                                      e.ID_Project == projectId
                                      select e);
                foreach (Product_License lic in dbEntitlements)
                {
                    entitlements.Add(lic.ToEEntitlement());
                }
            }
            return entitlements;
        }

        public List<EEntitlement> GetEntitlementsForClient(decimal clientId)
        {
            if (clientId < 1)
                throw new ArgumentNullException(nameof(clientId), "Id of the client must be greather than 0");

            List<EEntitlement> entitlements = new List<EEntitlement>();
            using (var context = _ContextGenerator.GenerateContext())
            {
                var dbEntitlements = (from e in context.Product_License
                                      where e.ID_Client == clientId
                                      select e);
                foreach (Product_License lic in dbEntitlements)
                {
                    entitlements.Add(lic.ToEEntitlement());
                }
            }
            return entitlements;
        }
    }
}