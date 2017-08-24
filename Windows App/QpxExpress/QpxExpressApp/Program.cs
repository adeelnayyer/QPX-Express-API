using QpxExpress.Services;
using System;
using System.Collections.Generic;

namespace QpxExpressApp
{
    class Program
    {
        static void Main()
        {
            StartProcess();

            Console.WriteLine("Completed !!!");
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
                    new TripFareService().FetchTripFares().Wait();
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
