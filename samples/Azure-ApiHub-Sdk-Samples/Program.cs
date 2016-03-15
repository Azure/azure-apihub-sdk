using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Management;
using Microsoft.Azure.ApiHub;
using System.IO;

namespace Azure.ApiHub.Sdk.Samples
{
    class Program
    {
        private const string subscriptionId = "83e6374a-dfa5-428b-82ef-eab6c6bdd383";
        private const string location ="brazilsouth";

        // look at armclient token to login to AAD and get arm token
        private static string aadToken = "";

        static void Main(string[] args)
        {
            if (args.Count() < 1)
            {
                Console.WriteLine("AAD token is missing.");
                return;
            }

            aadToken = args[0];

            // ListAllApisAsync(aadToken).Wait();
            GetConnectionKeyAsync(aadToken).Wait();

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        private static async Task ListAllApisAsync(string aadToken)
        {
            var hub = new ApiHubClient(subscriptionId, location, aadToken);

            // get all Managed APIs
            var apis = await hub.GetManagedApis();
            foreach (var managedApi in apis)
            {
                Console.WriteLine("Connections for {0}", managedApi.Name);
                Console.WriteLine("\tId:{0}", managedApi.Id);

                var connections = await hub.GetConnectionsAsync(managedApi.Name);
                foreach (var conn in connections)
                {
                    Console.WriteLine("\t\tName:{0}", conn.Name);
                }
            }
        }

        private static async Task GetConnectionKeyAsync(string aadToken)
        {
            var hub = new ApiHubClient(subscriptionId, location, aadToken);

            var connections = await hub.GetConnectionsAsync("dropbox");

            var connectionKey = await hub.GetConnectionKeyAsync(connections.First());

            var connectionString = hub.GetConnectionString(connectionKey.RuntimeUri, "Key", connectionKey.Key);
            var root = ItemFactory.Parse(connectionString);

            string cdpTestRoot = "cdpFiles/";
            var folder = await root.CreateFolderAsync(cdpTestRoot);

            await ListTestsAsync(cdpTestRoot, root, folder);

            await NonExistenceTestsAsync(cdpTestRoot, root, folder);

            await FileCrudTestsAsync(folder);

            await FileWatchTestsAsync(folder);
        }

        private static async Task FileWatchTestsAsync(IFolderItem folder)
        {
            bool fileAddedTrigger = false;
            bool fileUpdatedTrigger = false;

            var poll = folder.CreateNewFileWatcher(
                (fr) =>
                {
                    Console.WriteLine("File {0} was added.", fr.Path);
                    fileAddedTrigger = true;
                    return Task.FromResult(0);
                }, 1);

            var poll2 = folder.CreateUpdateFileWatcher(
                (fr) =>
                {
                    Console.WriteLine("File {0} was updated.", fr.Path);
                    fileUpdatedTrigger = true;
                    return Task.FromResult(0);
                }, 1);

            var newFile = await folder.CreateFileAsync(Guid.NewGuid().ToString());
            await newFile.WriteAsync(new byte[0]);

            // Adding some wait to make sure triggers are fired.
            await Task.Delay(1000);

            await newFile.WriteAsync(new byte[1] { 0 });

            // Adding some wait to make sure triggers are fired.
            await Task.Delay(20000);

            if (!fileAddedTrigger)
            {
                Console.WriteLine("Error: New file was not triggered when a new file was added.");
            }

            if (!fileUpdatedTrigger)
            {
                Console.WriteLine("Error: Update file was not triggered when an existing file was updated.");
            }

            await newFile.DeleteAsync();
        }

        private static async Task FileCrudTestsAsync(IFolderItem folder)
        {
            var file = await folder.CreateFileAsync(Guid.NewGuid().ToString() + ".txt", true);

            await file.WriteAsync(Encoding.Default.GetBytes(DateTime.Now.ToString()));

            Console.WriteLine("Created file Id: {0}", file.HandleId.Result);

            var content = Encoding.Default.GetString(await file.ReadAsync());

            Console.WriteLine("File content: " + content);

            var metadata = await file.GetMetadataAsync();
            Console.WriteLine("File last modified date/time " + metadata.LastModified.ToUniversalTime());

            await Task.Delay(1000);

            await file.WriteAsync(Encoding.Default.GetBytes(DateTime.Now.ToString()));

            metadata = await file.GetMetadataAsync();

            Console.WriteLine("File {0} updated succesfully!", metadata.Path);

            content = Encoding.Default.GetString(await file.ReadAsync());

            Console.WriteLine("Updated File content: " + content);

            Console.WriteLine("File last modified date/time " + metadata.LastModified.ToUniversalTime());

            await file.DeleteAsync();

            var items = await folder.ListAsync();

            foreach (var item in items)
            {
                if (item.Path.Equals(file.Path, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("File was not deleted successfully!");
                    break;
                }
            }
        }

        private static async Task ListTestsAsync(string cdpTestRoot, IFolderItem root, IFolderItem folder)
        {
            var currentList = await folder.ListAsync(false);
            Console.WriteLine("Number of items in {0} : {1}", folder.Path, currentList.Count());

            currentList = await folder.ListAsync(true);
            Console.WriteLine("Number of items in {0} and all its subfolders: {1}", folder.Path, currentList.Count());

            var nestedFolder = await folder.CreateFolderAsync("nestedFolder");

            currentList = await nestedFolder.ListAsync(false);
            Console.WriteLine("Number of items in {0} : {1}", nestedFolder.Path, currentList.Count());

            currentList = await nestedFolder.ListAsync(true);
            Console.WriteLine("Number of items in {0} and all its subfolders: {1}", nestedFolder.Path, currentList.Count());

            folder = await root.CreateFolderAsync(cdpTestRoot);
            var NestedFolderTwoLevel = await folder.GetFolderItemAsync("nestedFolder/nested2");
            currentList = await NestedFolderTwoLevel.ListAsync(true);
            Console.WriteLine("Number of items in {0} and all its subfolders: {1}", NestedFolderTwoLevel.Path, currentList.Count());

            currentList = await root.ListAsync(false);

            Console.WriteLine("Number of items in root folder {0} : {1}", root.Path, currentList.Count());
        }

        private static async Task NonExistenceTestsAsync(string cdpTestRoot, IFolderItem root, IFolderItem folder)
        {
            // file doesn't exist test 
            var fileItem = await folder.GetFileItemAsync(Guid.NewGuid().ToString());

            if(fileItem != null)
            {
                Console.WriteLine("Error: Files which do not exist should return null when calling GetFileItemAsync");
            }

            IFolderItem nullFolder = await folder.CreateFolderAsync(string.Empty);

            if(nullFolder != null)
            {
                Console.WriteLine("Error: null folder expected.");
            }

            var nullFile = await folder.CreateFileAsync(string.Empty);

            if (nullFile != null)
            {
                Console.WriteLine("Error: null file expected.");
            }

            var newFolder = await folder.CreateFolderAsync(Guid.NewGuid().ToString());

            // listing items for a folder which doesn't exist
            var listItems = await newFolder.ListAsync();

            if(listItems.Length > 0)
            {
                Console.WriteLine("Error: A folder which doesn't exist returned content.");
            }

            var newFile = await newFolder.CreateFileAsync(Guid.NewGuid().ToString());

            try
            {
                // trying to read from a file which doesn't exist should throw an exception.
                var bytes = await newFile.ReadAsync();
            }
            catch(FileNotFoundException ex)
            {
                Console.WriteLine("Expected exception was thrown when attempting to read from a file which doesn't exist.");
            }

            var metadata = await newFile.GetMetadataAsync();

            if(metadata != null)
            {
                Console.WriteLine("Error: Metadata should be null for a file which doesn't exist.");
            }

            // Deleting for a file which doesn't exist should do nothing.
            await newFile.DeleteAsync();

            // Writing null or empty should create an empty file
            await newFile.WriteAsync(null);

            metadata = await newFile.GetMetadataAsync();

            if(metadata == null || metadata.Size > 0)
            {
                Console.WriteLine("Error: Unable to create an empty file.");
            }

            await newFile.WriteAsync(new byte[] { 0 });

            metadata = await newFile.GetMetadataAsync();

            listItems =  await newFolder.ListAsync();

            if(listItems.Length != 1)
            {
                Console.WriteLine("Error: there should only be one item in the directory.");
            }

            await newFile.DeleteAsync();

            listItems = await newFolder.ListAsync();

            if (listItems.Length != 0)
            {
                Console.WriteLine("Error: there should be no items in the directory.");
            }

            string newFolderName = Guid.NewGuid().ToString();
            string newFileName = Guid.NewGuid().ToString();

            if(folder.FolderExists(newFolderName))
            {
                Console.WriteLine("Error: Folder must not yet exist.");
            }

            if (folder.FileExists(newFileName))
            {
                Console.WriteLine("Error: File must not yet exist.");
            }

            newFile = await folder.CreateFileAsync(newFolderName + "/" + newFileName);
            await newFile.WriteAsync(null);

            if (!folder.FolderExists(newFolderName))
            {
                Console.WriteLine("Error: Folder must now exist.");
            }

            if (!folder.FileExists(newFolderName + "/" + newFileName))
            {
                Console.WriteLine("Error: File must now exist.");
            }

            await newFile.DeleteAsync();

            // TODO: Cdp needs to support deleting empty folders.
        }
    }
}
