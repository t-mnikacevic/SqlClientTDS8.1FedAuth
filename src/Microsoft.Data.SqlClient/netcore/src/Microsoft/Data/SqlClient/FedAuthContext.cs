using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.Data.ProviderBase;

namespace Microsoft.Data.SqlClient
{
    sealed internal class FedAuthContext
    {
        private readonly SqlFedAuthInfo _fedAuthInfo;
        private DbConnectionPoolAuthenticationContext _fedAuthToken;

        public FedAuthContext(SqlFedAuthInfo fedAuthInfo)
        {
            _fedAuthInfo = fedAuthInfo;
        }
        internal SqlFedAuthInfo FedAuthInfo
        {
            get
            {
                return _fedAuthInfo;
            }
        }
        internal DbConnectionPoolAuthenticationContext FedAuthToken
        {
            get
            {
                return _fedAuthToken;
            }
            set
            {
                _fedAuthToken = value;
            }
        }
    }
}
