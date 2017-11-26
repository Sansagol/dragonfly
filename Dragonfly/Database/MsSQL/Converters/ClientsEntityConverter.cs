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
                Details = license.Details,
                ClientId = license.ID_Client,
                ProjectId = license.ID_Project,
                LicenseTypeId = license.ID_License_Type,
                UserCreatorId = license.ID_User_Creator
            };
        }

        public static Product_License ToProductLicense(this EEntitlement entitlement)
        {
            if (entitlement == null)
                return null;
            return new Product_License()
            {
                ID_Product_License = entitlement.Id >0 ? entitlement.Id : 0,
                Date_Begin = entitlement.DateBegin,
                Date_End = entitlement.DateEnd,
                Date_Created = entitlement.DateCreated,
                License_Count = entitlement.LicensesCount,
                Details = entitlement.Details,
                ID_Client = entitlement.ClientId,
                ID_License_Type = entitlement.LicenseTypeId,
                ID_Project = entitlement.ProjectId
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