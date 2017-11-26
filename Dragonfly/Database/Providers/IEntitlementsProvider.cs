using Dragonfly.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.Providers
{
    public interface IEntitlementsProvider: IDataProvider
    {
        EEntitlement GetEntitlement(decimal entitlementId);

        bool SaveEntitlement(EEntitlement entitlementToSave, decimal ownerId);

        /// <summary>Retrieve all license types.</summary>
        /// <returns>License types.</returns>
        List<ELicenseType> GetLicenseTypes();
    }
}