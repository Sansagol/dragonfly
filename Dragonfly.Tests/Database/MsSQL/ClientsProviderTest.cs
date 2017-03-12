using System;
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
            MsSqlFactory factory = new MsSqlFactory();
            return factory.CreateClientsProvider(Common.Connectionconfig);
        }

        [TestMethod]
        public void CreateClientTypeTest()
        {
            DragonflyEntities context = null;
            IClientsProvider provider = InitProvider();
            try
            {
                context = provider.Context as DragonflyEntities;
                context.Client_Type.RemoveRange(context.Client_Type);
                decimal id = provider.CreateAClientType("Test client type");
                Assert.IsTrue(id > 0, "Bad id returned.");
                var type = (from c in context.Client_Type
                            where c.ID_Client_Type == id
                            select c).FirstOrDefault();
                Assert.IsNotNull(type, $"Unable to retrieve created client type with id {id}");
            }
            finally
            {
                context?.Client_Type.RemoveRange(context.Client_Type);
                provider?.Dispose();
            }
        }

        [TestMethod]
        public void CreateClientTest()
        {
            var provider = InitProvider();
            var context = provider.Context as DragonflyEntities;
            try
            {
                context.Client.RemoveRange(context.Client);
                context.Client_Type.RemoveRange(context.Client_Type);
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
                context?.Client.RemoveRange(context.Client);
                context?.Client_Type.RemoveRange(context.Client_Type);
                provider?.Dispose();
            }
        }

        private static ClientType CreateClientType(IClientsProvider provider, DragonflyEntities context)
        {
            decimal id = provider.CreateAClientType("Test client type");
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
            var context = provider.Context as DragonflyEntities;
            try
            {
                context.Client.RemoveRange(context.Client);
                context.Client_Type.RemoveRange(context.Client_Type);
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
                context?.Client.RemoveRange(context.Client);
                context?.Client_Type.RemoveRange(context.Client_Type);
                provider?.Dispose();
            }
        }


        [TestMethod]
        public void GetAllAvailableTypesTest()
        {
            var provider = InitProvider();
            var context = provider.Context as DragonflyEntities;
            try
            {
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
                context?.Client_Type.RemoveRange(context.Client_Type);
                provider?.Dispose();
            }
        }
    }
}