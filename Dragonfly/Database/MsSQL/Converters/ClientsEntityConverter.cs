using Dragonfly.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL.Converters
{
    static class ClientsEntityConverter
    {
        public static EEntitlement ToEEntitlement(this Product_License license)
        {
            if (license == null)
                return null;
            return new EEntitlement()
            {
                Id = license.ID_Product_License,
                DateBegin = license.Date_Begin,
                DateEnd = license.Date_End,
                DateCreated = license.Date_Created,
                LicensesCount = license.License_Count,
                Details = license.Details
            };
        }

        public static ELicenseType ToELicenseType(this License_Type type)
        {
            if (type == null)
                return null;

            return new ELicenseType()
            {
                Id = type.ID_License_Type,
                DateCreated = type.Date_Created,
                Name = type.Type_Name
            };
        }

        public static EClient ToEClient(this Client client)
        {
            if (client == null)
                return null;
            EClient model = new EClient()
            {
                Id = client.ID_Client,
                Name = client.Name,
                INN = client.INN,
                OGRN = client.OGRN,
                KPP = client.KPP,
                InnerName = client.Inner_Name                
            };
            return model;
        }
    }
}