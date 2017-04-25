using Dragonfly.Core.Settings;
using Dragonfly.Database.MsSQL.LowLevel;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL
{
    /// <summary>
    /// Base class of data MS SQL data providers.
    /// </summary>
    class DataProvider 
    {
        protected IDBContextGenerator _ContextGenerator = null;

        public DataProvider(IDBContextGenerator contextgenerator)
        {
            if (contextgenerator == null)
                throw new ArgumentNullException(nameof(contextgenerator));

            _ContextGenerator = contextgenerator;
        }

        /// <summary>Method create and open context for database.</summary>
        /// <param name="accessConfigurations">Parameters to database connect.</param>
        /// <exception cref="DbInitializationException">Error on database initialization.</exception>
        public void Initialize()
        {
        }

        /// <summary>
        /// Hack method to generate the context of the database.
        /// </summary>
        /// <param name="accessConfigurations"></param>
        /// <returns></returns>
        public DragonflyEntities GenerateContext(DatabaseAccessConfiguration accessConfigurations)
        {
            return _ContextGenerator.GenerateContext(accessConfigurations);
        }
    }
}