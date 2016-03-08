using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microfoft.Azure.ApiHub.Sdk.Management;

namespace Azure.ApiHub.Sdk.Samples
{
    class Program
    {
        private const string subscriptionId = "83e6374a-dfa5-428b-82ef-eab6c6bdd383";
        private const string resourceGroup = "AzureFunctions";
        private const string location ="brazilsouth";

        // look at armclient token to login to AAD and get arm token
        private static string aadToken = "";

        static void Main(string[] args)
        {
            // ListAllApisAsync(args).Wait();
            GetConnectionKeyAsync(args).Wait();

            Console.ReadLine();
        }

        private static async Task ListAllApisAsync(string[] args)
        {
            aadToken = args[0];
            var hub = new ApiHubClient(subscriptionId, resourceGroup, location, aadToken);

            // get all Managed APIs
            var apis = await hub.GetManagedApis();
            foreach (var managedApi in apis)
            {
                Console.WriteLine("Connections for {0}", managedApi.Name);
                Console.WriteLine("\tId:{0}", managedApi.Id);

                var connections = await hub.GetConnections(managedApi.Name);
                foreach (var conn in connections)
                {
                    Console.WriteLine("\t\tName:{0}", conn.Name);
                }
            }
        }

        private static async Task GetConnectionKeyAsync(string[] args)
        {
            aadToken = args[0];
            var hub = new ApiHubClient(subscriptionId, resourceGroup, location, aadToken);

            var connections = await hub.GetConnections("dropbox");

            Tuple<string, Uri> connectionKeys = await hub.GetConnectionKey(connections.First());

            string connectonKey = connectionKeys.Item1;
            Uri runtimeUrl = connectionKeys.Item2;

            Console.WriteLine("Runtime Url: {0}", runtimeUrl);
        }
    }
}
