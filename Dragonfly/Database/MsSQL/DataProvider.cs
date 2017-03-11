using Dragonfly.Core.Settings;
using Dragonfly.Database.MsSQL.LowLevel;
using Dragonfly.Database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Database.MsSQL
{
    /// <summary>
    /// Base class of data MS SQL data providers.
    /// </summary>
    class DataProvider 
    {
        private IDBContextGenerator _ContextGenerator = null;

        protected DragonflyEntities _Context = null;

        public DataProvider(IDBContextGenerator contextgenerator)
        {
            if (contextgenerator == null)
                throw new ArgumentNullException(nameof(contextgenerator));

            _ContextGenerator = contextgenerator;
        }

        /// <summary>Method create and open context for database.</summary>
        /// <param name="accessConfigurations">Parameters to database connect.</param>
        /// <exception cref="DbInitializationException">Error on database initialization.</exception>
        public void Initialize(DatabaseAccessConfiguration accessConfigurations)
        {
            _Context = _ContextGenerator.GenerateContext(accessConfigurations);
        }

        public void Dispose()
        {
            _Context?.Dispose();
            _Context = null;
        }
    }
}