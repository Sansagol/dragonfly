﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragonfly.Database.MsSQL;
using System.Data.Entity;
using Dragonfly.Models.Clients;
using Dragonfly.Database.Providers;
using Dragonfly.Database;

namespace Dragonfly.Tests.Database.MsSQL
{
    [TestClass]
    public class ClientsProviderTest
    {
        /// <summary>
        /// Method initializes a database provider.
        /// </summary>
        /// <returns>Initialized provider.</returns>
        private IClientsProvider InitProvider()
        {
            MsSqlFactory factory = new MsSqlFactory(Common.Connectionconfig);
            return factory.CreateClientsProvider();
        }

        private void ClearResources(DragonflyEntities context)
        {
            context.Client.RemoveRange(context.Client);
            context.Client_Type.RemoveRange(context.Client_Type);
            context.SaveChanges();
        }

        [TestMethod]
        public void CreateClientTypeTest()
        {
            IClientsProvider provider = InitProvider();
            using (var context = ((DataProvider)provider).GenerateContext())
            {
                try
                {

                    ClearResources(context);
                    decimal id = provider.CreateAClientType("Test client type", 1);
                    Assert.AreEqual(1, id, $"Bad id returned: {id}.");
                    var type = (from c in context.Client_Type
                                where c.ID_Client_Type == id
                                select c).FirstOrDefault();
                    Assert.IsNotNull(type, $"Unable to retrieve created client type with id {id}");
                }
                finally
                {
                    ClearResources(context);
                }
            }
        }

        [TestMethod]
        public void CreateClientTest()
        {
            var provider = InitProvider();
            using (var context = ((DataProvider)provider).GenerateContext())
            {
                try
                {
                    ClearResources(context);
                    ClientType type = CreateClientType(provider, context);
                    var model = new ClientModel()
                    {
                        Name = "Name",
                        InnerName = "Inner name",
                        INN = "123",
                        OGRN = "456",
                        KPP = "111",
                        Type = type
                    };
                    provider.CreateClient(model);

                    Assert.IsTrue(model.ID > 0, "Id of the saved client is not set.");
                    var loadedClient = (from cli in context.Client
                                        where cli.ID_Client == model.ID
                                        select cli).FirstOrDefault();
                    Assert.IsNotNull(loadedClient, $"Client with id {model.ID} not found in Database.");
                    Assert.AreEqual(model.ID, loadedClient.ID_Client);
                    Assert.AreEqual(model.Name, loadedClient.Name);
                    Assert.AreEqual(model.InnerName, loadedClient.Inner_Name);
                    Assert.AreEqual(model.INN, loadedClient.INN);
                    Assert.AreEqual(model.OGRN, loadedClient.OGRN);
                    Assert.AreEqual(model.KPP, loadedClient.KPP);
                    Assert.AreEqual(model.Type.ID, loadedClient.ID_Client_Type);
                }
                finally
                {
                    ClearResources(context);
                }
            }
        }

        private static ClientType CreateClientType(IClientsProvider provider, DragonflyEntities context)
        {
            decimal id = provider.CreateAClientType("Test client type", 1);
            var type = (from c in context.Client_Type
                        where c.ID_Client_Type == id
                        select c).FirstOrDefault().ToClientType();
            return type;
        }

        [TestMethod]
        [ExpectedException(typeof(InsertDbDataException))]
        public void CreateDoubleClientTest()
        {
            var provider = InitProvider();
            using (var context = ((DataProvider)provider).GenerateContext())
            {
                try
                {
                    ClearResources(context);
                    ClientType type = CreateClientType(provider, context);

                    var model = new ClientModel()
                    {
                        Name = "Name",
                        Type = type
                    };
                    var model2 = new ClientModel()
                    {
                        Name = "Name",
                        Type = type
                    };
                    provider.CreateClient(model);
                    provider.CreateClient(model2);
                }
                finally
                {
                    ClearResources(context);
                }
            }
        }


        [TestMethod]
        public void GetAllAvailableTypesTest()
        {
            var provider = InitProvider();
            using (var context = ((DataProvider)provider).GenerateContext())
            {
                try
                {
                    ClearResources(context);
                    CreateClientType(provider, context);
                    var clientTypes = provider.GetAvailableClientTypes();
                    var realTypes = (from t in context.Client_Type
                                     select t);
                    Assert.AreEqual(realTypes.Count(), clientTypes.Count(),
                        "Collections have different number elements");
                    foreach (var realType in realTypes)
                    {
                        Assert.IsTrue(clientTypes.Any(t => t.ID == realType.ID_Client_Type));
                    }
                }
                finally
                {
                    ClearResources(context);
                }
            }
        }
    }
}