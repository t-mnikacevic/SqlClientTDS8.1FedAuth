
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

namespace Microsoft.Data.SqlClient
{
    class FedAuthCache
    {
        private static FedAuthCache instance;
        /// <summary>
        /// The private member which carries the set of authenticationcontexts for this pool (based on the user's identity).
        /// </summary>
        private readonly ConcurrentDictionary<SqlConnectionString, FedAuthContext> _cachedContexts;

        private FedAuthCache()
        {
            _cachedContexts = new ConcurrentDictionary<SqlConnectionString, FedAuthContext>(
                                                                               concurrencyLevel: 4 * Environment.ProcessorCount,
                                                                               capacity: 2
                                                                              );
        }

        /// <summary>
        /// Return the pooled authentication contexts.
        /// </summary>
        internal ConcurrentDictionary<SqlConnectionString, FedAuthContext> CachedContexts
        {
            get
            {
                return _cachedContexts;
            }
        }

        public static FedAuthCache Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FedAuthCache();
                }
                return instance;
            }
        }
    }
}
