using QpxExpress.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace QpxExpressApp
{
    class Program
    {

        static HttpClient client = new HttpClient();

        static void Main()
        {
            StartProcess();

            Console.WriteLine("Press any key to continue . . .");
            Console.ReadKey();
        }

        private static bool EnvironmentVariablesExist()
        {
            var requiredEnvironmentVariables = new List<string>
            {
                ConfigurationProvider.ApiKeyVariableName,
                ConfigurationProvider.ConnectionStringVariableName
            };

            var exists = true;
            foreach (var environmentVariable in requiredEnvironmentVariables)
            {
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(environmentVariable)))
                {
                    Console.WriteLine("'{0}' environment variable does not exist.", environmentVariable);
                    exists = false;
                }
            }

            return exists;
        }

        private static void StartProcess()
        {
            try
            {
                if (EnvironmentVariablesExist())
                {
                    new SearchTripsService().FetchTripFares().Wait();
                }
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
        }
    }
}
