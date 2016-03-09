using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microfoft.Azure.ApiHub.Sdk.Management;
using Microfoft.Azure.ApiHub.Sdk.Cdp;

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
            if(args.Count() < 1)
            {
                Console.WriteLine("AAD token is missing.");
                return;
            }

            aadToken = args[0];
            var hub = new ApiHubClient(subscriptionId, resourceGroup, location, aadToken);

            var connections = await hub.GetConnections("dropbox");

            Tuple<string, Uri> connectionKeys = await hub.GetConnectionKey(connections.First());

            string connectonKey = connectionKeys.Item1;
            Uri runtimeUrl = connectionKeys.Item2;

            string acsKey = string.Format("{0};Key;{1}", runtimeUrl, connectonKey);

            Console.WriteLine("acs Key: {0}", acsKey);

            var cdpConnectionString = CdpSource.GetConnectionString(runtimeUrl, "Key", connectonKey);

            IFileSource cdp = await CdpSource.ParseAsync(cdpConnectionString);

            string cdpTestRoot = "cdpFiles/";

            var item = await cdp.CreateAsync(cdpTestRoot + Guid.NewGuid().ToString() + ".txt", Encoding.Default.GetBytes(DateTime.Now.ToString()));

            Console.WriteLine("Created file name: {0}", item.Name);

            var metadata1 = await cdp.GetMetaDataAsync(cdpTestRoot + item.Name);
            var metadata2 = await cdp.GetMetaDataAsync(await cdp.GetIdAsync(cdpTestRoot + item.Name));

            if(item.Name != metadata1.Name || item.Name != metadata2.Name)
            {
                Console.WriteLine("An error occured. metadata info don't match!");
            }

            var content1 = Encoding.Default.GetString(await cdp.ReadAsync(cdpTestRoot + item.Name));
            var content2 = Encoding.Default.GetString(await cdp.ReadAsync(await cdp.GetIdAsync(cdpTestRoot + item.Name)));

            if( content1 != content2)
            {
                Console.WriteLine("An error occured. metadata info don't match!");
            }
            else
            {
                Console.WriteLine("File content: " + content1);
            }

            await Task.Delay(1000);

            var updatedItem = await cdp.UpdateAsync(await cdp.GetIdAsync(cdpTestRoot + item.Name), Encoding.Default.GetBytes(DateTime.Now.ToString()));

            if(updatedItem.LastModified.ToUniversalTime() <= item.LastModified.ToUniversalTime())
            {
                Console.WriteLine("File update for {0} failed!" , item.Name);
            }
            else
            {
                Console.WriteLine("File {0} updated succesfully!" , item.Name);
            }

            await cdp.DeleteAsync(await cdp.GetIdAsync(cdpTestRoot + item.Name));

            var deletedItem = await cdp.GetIdAsync(cdpTestRoot + item.Name);

            if(deletedItem != null)
            {
                Console.WriteLine("File delete for {0} failed!" , item.Name);
            }
            else
            {
                Console.WriteLine("File {0} deleted succesfully!" , item.Name);
            }

            var items = await cdp.ListAsync(null, false);
            Console.WriteLine("Number of files in the root: " + items.Count());

            items = await cdp.ListAsync(items[0], true);
            Console.WriteLine("Number of files: " + items.Count());

            var folder = "cdpfiles/nestedfolder";
            var poll = cdp.CreateNewFileWatcher(folder,
                (fr) =>
                {
                    Console.WriteLine("File {0} was added." , fr.Name);
                    return Task.FromResult(0);
                });

            Console.WriteLine("Waiting for files to be added... Press Enter to continue");
            Console.ReadLine();

            //var folder = "test1";
            //var poll = cdp.CreateUpdateFileWatcher(folder,
            //    (fr) =>
            //    {
            //        Console.WriteLine(fr.Name);
            //        return Task.FromResult(0);
            //    });
            //Console.ReadLine();
        }
    }
}
