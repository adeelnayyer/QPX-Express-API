using System;

namespace QpxExpress.Services
{
    public class ConfigurationProvider
    {
        public static string ConnectionString
        {
            get { return Environment.GetEnvironmentVariable(ConnectionStringVariableName); }
        }

        public static string ApiKeyVariableName
        {
            get { return "QpxExpressApiKey"; }
        }

        public static string ConnectionStringVariableName
        {
            get { return "QpxExpressConnectionString"; }
        }
    }
}
