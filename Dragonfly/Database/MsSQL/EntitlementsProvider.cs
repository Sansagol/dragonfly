using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dragonfly.Database.Entities;
using Dragonfly.Database.MsSQL.LowLevel;
using Dragonfly.Database.MsSQL.Converters;

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
            throw new NotImplementedException();
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

        public bool SaveEntitlement(EEntitlement entitlementToSave)
        {
            throw new NotImplementedException();
        }
    }
}