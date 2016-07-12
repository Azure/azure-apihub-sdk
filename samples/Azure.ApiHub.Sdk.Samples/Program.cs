using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Management;
using Microsoft.Azure.ApiHub;
using System.IO;
using Microsoft.Azure.ApiHub.Sdk;
using Microsoft.Azure.ApiHub.Sdk.Common;
using Microsoft.Azure.ApiHub.Sdk.Table;
using Newtonsoft.Json.Linq;

namespace Azure.ApiHub.Sdk.Samples
{
    class Program
    {
        private const string subscriptionId = "83e6374a-dfa5-428b-82ef-eab6c6bdd383";
        private const string location = "brazilsouth";

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

            //ListAllApisAsync(aadToken).Wait();

            //GetConnectionKeyAsync(aadToken).Wait();

            RunApiHubTests("UseLocalFileSystem=true;Path=C:\\tests").Wait();

            //ReadFromWriteToSaasProvidersTestAsync(aadToken, "dropbox", "googledrive").Wait();

            //TableClientTestAsync().Wait();

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        public class SampleEntity
        {
            public int Id { get; set; }
            public string Text { get; set; }
        }

        // NOTE: Prerequsites for running this sample:
        // 1. Create SampleTable in your SQL database:
        //      CREATE TABLE SampleTable
        //      (
        //          Id int NOT NULL,
        //          Text nvarchar(10) NULL
        //          CONSTRAINT PK_Id PRIMARY KEY(Id)
        //      )
        // 2. Provide valid endpoint, scheme, accessToken values 
        // in the connection string below.
        private static async Task TableClientTestAsync()
        {
            const string connectionStringFormat = "endpoint={0};scheme={1};accesstoken={2}";

            var connectionString = string.Format(connectionStringFormat,
                "Valid endpoint goes here",
                "Valid scheme goes here",
                "Valid accessToken goes here");

            var connection = new Connection(connectionString);
            var tableClient = connection.CreateTableClient();

            var datasetsSegment = await tableClient.ListDataSetsAsync();

            foreach (var dataset in datasetsSegment.Items)
            {
                Console.WriteLine(dataset.DataSetName);

                var tablesSegment = await dataset.ListTablesAsync();

                foreach (var table in tablesSegment.Items)
                {
                    Console.WriteLine(table.TableName);

                    var metadata = await table.GetMetadataAsync();

                    Console.WriteLine(metadata.Schema.ToString());

                    ContinuationToken continuationToken = null;
                    do
                    {
                        var entitiesSegment = await table.ListEntitiesAsync(
                            continuationToken: continuationToken);

                        foreach (var entity in entitiesSegment.Items)
                        {
                            foreach (var kvp in entity)
                            {
                                Console.WriteLine(kvp.Key + ": " + kvp.Value);
                            }
                        }

                        continuationToken = entitiesSegment.ContinuationToken;
                    }
                    while (continuationToken != null);

                    var newEntity = new JObject();
                    newEntity["Id"] = 2;
                    newEntity["Text"] = "foo";

                    //await table.CreateEntityAsync(newEntity);

                    //await table.UpdateEntityAsync("2", newEntity);

                    //await table.DeleteEntityAsync("2");
                }
            }

            //var table1 = tableClient.GetDataSetReference().GetTableReference<SampleEntity>("SampleTable");
            //var entity1 = await table1.GetEntityAsync("1");
        }

        private static async Task ReadFromWriteToSaasProvidersTestAsync(string aadToken, string source, string destination)
        {
            var hub = new ApiHubClient(subscriptionId, location, aadToken);

            var connections = await hub.GetConnectionsAsync(source);
            var connectionKey = await hub.GetConnectionKeyAsync(connections.First());
            var connectionString = hub.GetConnectionString(connectionKey.RuntimeUri, "Key", connectionKey.Key);
            var sourceRoot = ItemFactory.Parse(connectionString);

            connections = await hub.GetConnectionsAsync(destination);
            connectionKey = await hub.GetConnectionKeyAsync(connections.First());
            connectionString = hub.GetConnectionString(connectionKey.RuntimeUri, "Key", connectionKey.Key);
            var destinationRoot = ItemFactory.Parse(connectionString);

            string fileName = "test/aaa.txt";

            var sourceFile = sourceRoot.GetFileReference(fileName);
            await sourceFile.WriteAsync(Encoding.Default.GetBytes(DateTime.Now.ToString()));
            var sourceContent = await sourceFile.ReadAsync();

            var destinationFile = destinationRoot.GetFileReference(fileName);
            await destinationFile.WriteAsync(sourceContent);
            var destinationContent = await destinationFile.ReadAsync();

            if ((Encoding.Default.GetString(sourceContent)).Equals((Encoding.Default.GetString(destinationContent))))
            {
                Console.WriteLine("Source and destination files match!");
            }
            else
            {
                Console.WriteLine("Error: Source and destination files do not match!");
            }
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

            await RunApiHubTests(connectionString);
        }

        private static async Task RunApiHubTests(string connectionString)
        {
            var logger = new ConsoleLogger();
            var root = ItemFactory.Parse(connectionString, logger);

            string cdpTestRoot = "cdpFiles/nested/";
            var folder = root.GetFolderReference(cdpTestRoot);

            await ListTestsAsync(cdpTestRoot, root, folder);

            await NonExistenceTestsAsync(cdpTestRoot, root, folder);

            await FileCrudTestsAsync(folder);

            await FileWatchTestsAsync(folder);
        }

        private static async Task FileWatchTestsAsync(IFolderItem folder)
        {
            bool fileAddedTrigger = false;
            bool fileUpdatedTrigger = false;

            var poll = folder.CreateFileWatcher(FileWatcherType.Created,
                (fr, obj) =>
                {
                    Console.WriteLine("File {0} was added.", fr.Path);

                    var uri = obj as Uri;
                    if (uri != null)
                    {
                        Console.WriteLine("Next Uri: {0}", uri.AbsolutePath);
                    }
                    fileAddedTrigger = true;
                    return Task.FromResult(0);
                }, null, 1);

            var poll2 = folder.CreateFileWatcher(FileWatcherType.Updated,
                (fr, obj) =>
                {
                    Console.WriteLine("File {0} was updated.", fr.Path);

                    var uri = obj as Uri;
                    if (uri != null)
                    {
                        Console.WriteLine("Next Uri: {0}", uri.AbsolutePath);
                    }
                    fileUpdatedTrigger = true;
                    return Task.FromResult(0);
                }, null, 1);

            var newFile = folder.GetFileReference(Guid.NewGuid().ToString());
            await newFile.WriteAsync(new byte[0]);

            // Adding some wait to make sure triggers are fired.
            await Task.Delay(1000);

            await newFile.WriteAsync(new byte[1] { 0 });

            // Adding some wait to make sure triggers are fired.
            Console.WriteLine("Waiting for 20 seconds...");
            await Task.Delay(20000);

            if (!fileAddedTrigger)
            {
                Console.WriteLine("Error: New file was not triggered when a new file was added.");
            }

            if (!fileUpdatedTrigger)
            {
                Console.WriteLine("Error: Update file was not triggered when an existing file was updated.");
            }

            var trigger = await folder.CheckForFileAsync(FileWatcherType.Updated);
            await newFile.WriteAsync(new byte[1] { 1 });

            trigger = await folder.CheckForFileAsync(FileWatcherType.Updated, trigger.NextUri);

            if (trigger.FileItem != null)
            {
                Console.WriteLine("Updated file: {0}", trigger.FileItem.Path);
            }

            await newFile.DeleteAsync();
        }

        private static async Task FileCrudTestsAsync(IFolderItem folder)
        {
            var file = folder.GetFileReference(Guid.NewGuid().ToString() + ".txt", true);

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

            var nestedFolder = folder.GetFolderReference("nestedFolder");

            currentList = await nestedFolder.ListAsync(false);
            Console.WriteLine("Number of items in {0} : {1}", nestedFolder.Path, currentList.Count());

            currentList = await nestedFolder.ListAsync(true);
            Console.WriteLine("Number of items in {0} and all its subfolders: {1}", nestedFolder.Path, currentList.Count());

            folder = root.GetFolderReference(cdpTestRoot);
            var NestedFolderTwoLevel = await folder.GetFolderItemAsync("nestedFolder/nested2");

            if (NestedFolderTwoLevel != null)
            {
                currentList = await NestedFolderTwoLevel.ListAsync(true);
                Console.WriteLine("Number of items in {0} and all its subfolders: {1}", NestedFolderTwoLevel.Path, currentList.Count());
            }

            currentList = await root.ListAsync(false);

            Console.WriteLine("Number of items in root folder {0} : {1}", root.Path, currentList.Count());
        }

        private static async Task NonExistenceTestsAsync(string cdpTestRoot, IFolderItem root, IFolderItem folder)
        {
            // file doesn't exist test 
            var fileItem = await folder.GetFileItemAsync(Guid.NewGuid().ToString());

            if (fileItem != null)
            {
                Console.WriteLine("Error: Files which do not exist should return null when calling GetFileItemAsync");
            }

            IFolderItem nullFolder = folder.GetFolderReference(string.Empty);

            if (nullFolder != null)
            {
                Console.WriteLine("Error: null folder expected.");
            }

            var nullFile = folder.GetFileReference(string.Empty);

            if (nullFile != null)
            {
                Console.WriteLine("Error: null file expected.");
            }

            var newFolder = folder.GetFolderReference(Guid.NewGuid().ToString());

            // listing items for a folder which doesn't exist
            var listItems = await newFolder.ListAsync();

            if (listItems.Length > 0)
            {
                Console.WriteLine("Error: A folder which doesn't exist returned content.");
            }

            var newFile = newFolder.GetFileReference(Guid.NewGuid().ToString());

            try
            {
                // trying to read from a file which doesn't exist should throw an exception.
                var bytes = await newFile.ReadAsync();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Expected exception was thrown when attempting to read from a file which doesn't exist.");
            }

            var metadata = await newFile.GetMetadataAsync();

            if (metadata != null)
            {
                Console.WriteLine("Error: Metadata should be null for a file which doesn't exist.");
            }

            // Deleting for a file which doesn't exist should do nothing.
            await newFile.DeleteAsync();

            // Writing null or empty should create an empty file
            await newFile.WriteAsync(null);

            metadata = await newFile.GetMetadataAsync();

            if (metadata == null || metadata.Size > 0)
            {
                Console.WriteLine("Error: Unable to create an empty file.");
            }

            await newFile.WriteAsync(new byte[] { 0 });

            metadata = await newFile.GetMetadataAsync();

            listItems = await newFolder.ListAsync();

            if (listItems.Length != 1)
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

            if (await folder.FolderExistsAsync(newFolderName))
            {
                Console.WriteLine("Error: Folder must not yet exist.");
            }

            if (await folder.FileExistsAsync(newFileName))
            {
                Console.WriteLine("Error: File must not yet exist.");
            }

            newFile = folder.GetFileReference(newFolderName + "/" + newFileName);
            await newFile.WriteAsync(null);

            if (!await folder.FolderExistsAsync(newFolderName))
            {
                Console.WriteLine("Error: Folder must now exist.");
            }

            if (!await folder.FileExistsAsync(newFolderName + "/" + newFileName))
            {
                Console.WriteLine("Error: File must now exist.");
            }

            await newFile.DeleteAsync();

            // TODO: Cdp needs to support deleting empty folders.
        }
    }
}
