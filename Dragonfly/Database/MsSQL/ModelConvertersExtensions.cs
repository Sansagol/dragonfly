using Dragonfly.Models.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL
{
    /// <summary>
    /// Class contains extension methods to convert an model objects
    /// to an entityModel objects.
    /// </summary>
    public static class ModelConvertersExtensions
    {
        public static Client ToClient(this ClientModel model)
        {
            Client cli = new Client()
            {
                ID_Client = model.ID,
                INN = model.INN,
                OGRN = model.OGRN,
                KPP = model.KPP,
                Name = model.Name,
                Inner_Name = model.InnerName,
                ID_Client_Type = model.Type.ID
            };
            return cli;
        }
    }
}