// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Azure.ApiHub.Tests.Cdp
{
    public class CdpEndToEndTests
    {
        private string _apiHubConnectionString;
        private IFolderItem _rootFolder;

        public CdpEndToEndTests()
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AzureWebJobsDropBox")))
            {
                _apiHubConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsDropBox");
            }
            else
            {
                _apiHubConnectionString = "UseLocalFileSystem=true;Path=" + Path.GetTempPath() + "ApiHubDropBox";
            }

            _rootFolder = ItemFactory.Parse(_apiHubConnectionString);
        }

        [Fact]
        public async Task ListTestsAsync()
        {
            string cdpTestRoot = "tests/" + Guid.NewGuid().ToString();
            string fileName = Guid.NewGuid().ToString();

            var folder = await _rootFolder.GetFolderReferenceAsync(cdpTestRoot);
            var file = await folder.GetFileReferenceAsync(fileName);
            await file.WriteAsync(new byte[] { });

            IItem[] currentList = await folder.ListAsync(false);
            Assert.Equal(currentList.Length, 1);

            currentList = await folder.ListAsync(true);
            Assert.Equal(currentList.Length, 1);

            var nestedFolder = await folder.GetFolderReferenceAsync("nestedFolder");
            var file2 = await nestedFolder.GetFileReferenceAsync(fileName);
            await file2.WriteAsync(new byte[] { });

            currentList = await nestedFolder.ListAsync(true);
            Assert.Equal(currentList.Length, 1);

            currentList = await folder.ListAsync(true);
            Assert.Equal(currentList.Length, 3);

            await file2.DeleteAsync();

            currentList = await folder.ListAsync(true);
            Assert.Equal(currentList.Length, 2);

            await file.DeleteAsync();

            currentList = await folder.ListAsync(true);
            Assert.Equal(currentList.Length, 1);
        }

        [Fact]
        public async Task FileCrudTestsAsync()
        {
            string cdpTestRoot = "tests/" + Guid.NewGuid().ToString();
            string fileName = Guid.NewGuid().ToString();

            var folder = await _rootFolder.GetFolderReferenceAsync(cdpTestRoot);

            var file = await folder.GetFileReferenceAsync(fileName, true);

            var content = "test\r\n";
            await file.WriteAsync(Encoding.Default.GetBytes(content));

            Assert.True(file.Path.Contains(fileName));
            Assert.True(file.Path.Contains(cdpTestRoot));

            var retrievedContent = Encoding.Default.GetString(await file.ReadAsync());

            Assert.Equal(content, retrievedContent);

            var metadata = await file.GetMetadataAsync();

            Assert.Equal(metadata.IsFolder, false);

            var currentLastModified = metadata.LastModified.ToUniversalTime();

            await Task.Delay(2000);

            content += "  Updated";
            await file.WriteAsync(Encoding.Default.GetBytes(content));

            var updatedLastModified = (await file.GetMetadataAsync()).LastModified.ToUniversalTime();

            Assert.True(updatedLastModified > currentLastModified);

            retrievedContent = Encoding.Default.GetString(await file.ReadAsync());

            Assert.Equal(content, retrievedContent);

            await file.DeleteAsync();
        }

        [Fact]
        public async Task FileWatchTestsAsync()
        {
            bool fileAddedTrigger = false;
            bool fileUpdatedTrigger = false;

            string cdpTestRoot = "tests/" + Guid.NewGuid().ToString();

            var folder = await _rootFolder.GetFolderReferenceAsync(cdpTestRoot);

            // Create an empty file to create an empty new folder on SAAS provider
            var initialFile = await folder.GetFileReferenceAsync(Guid.NewGuid().ToString());

            await initialFile.WriteAsync(new byte[0]);

            string fileName = Guid.NewGuid().ToString();

            var poll = folder.CreateNewFileWatcher(
                (fr) =>
                {
                    Assert.True(fr.Path.Contains(fileName));
                    fileAddedTrigger = true;
                    return Task.FromResult(0);
                }, 1);

            var poll2 = folder.CreateUpdateFileWatcher(
                (fr) =>
                {
                    Assert.True(fr.Path.Contains(fileName));
                    fileUpdatedTrigger = true;
                    return Task.FromResult(0);
                }, 1);


            var newFile = await folder.GetFileReferenceAsync(fileName);
            await newFile.WriteAsync(new byte[0]);

            // Adding some wait to make sure triggers are fired.
            await Task.Delay(10000);

            await newFile.WriteAsync(new byte[1] { 0 });

            // Adding some wait to make sure triggers are fired.
            await Task.Delay(10000);

            Assert.True(fileAddedTrigger);
            Assert.True(fileUpdatedTrigger);

            await newFile.DeleteAsync();
            await initialFile.DeleteAsync();
        }

        [Fact]
        public async Task NonExistenceTestsAsync()
        {
            string cdpTestRoot = "tests/" + Guid.NewGuid().ToString();
            string fileName = Guid.NewGuid().ToString();

            var folder = await _rootFolder.GetFolderReferenceAsync(cdpTestRoot);

            // file doesn't exist test 
            var fileItem = await folder.GetFileItemAsync(Guid.NewGuid().ToString());

            Assert.Null(fileItem);

            IFolderItem nullFolder = await folder.GetFolderReferenceAsync(string.Empty);

            Assert.Null(nullFolder);

            var nullFile = await folder.GetFileReferenceAsync(string.Empty);

            Assert.Null(nullFile);

            var newFolder = await folder.GetFolderReferenceAsync(Guid.NewGuid().ToString());

            // listing items for a folder which doesn't exist
            var listItems = await newFolder.ListAsync();

            Assert.Equal(listItems.Length, 0);

            var newFile = await newFolder.GetFileReferenceAsync(Guid.NewGuid().ToString());

            try
            {
                // trying to read from a file which doesn't exist should throw an exception.
                var bytes = await newFile.ReadAsync();
            }
            catch (Exception ex)
            {
                // Expected exception was thrown when attempting to read from a file which doesn't exist.
                Assert.IsType(typeof(FileNotFoundException), ex);
            }
            
            var metadata = await newFile.GetMetadataAsync();

            // Metadata should be null for a file which doesn't exist.
            Assert.Null(metadata);

            // Deleting for a file which doesn't exist should do nothing.
            await newFile.DeleteAsync();

            // Writing null or empty should create an empty file
            await newFile.WriteAsync(null);

            metadata = await newFile.GetMetadataAsync();

            // empty file should have size of 0
            Assert.Equal(metadata.Size, 0);

            await newFile.WriteAsync(new byte[] { 0 });
            metadata = await newFile.GetMetadataAsync();

            Assert.Equal(metadata.Size, 1);

            string newFolderName = Guid.NewGuid().ToString();
            string newFileName = Guid.NewGuid().ToString();

            Assert.False(folder.FolderExists(newFolderName));
            Assert.False(folder.FileExists(newFileName));

            newFile = await folder.GetFileReferenceAsync(newFolderName + "/" + newFileName);
            await newFile.WriteAsync(null);

            Assert.True(folder.FolderExists(newFolderName));
            Assert.True(folder.FileExists(newFolderName + "/" + newFileName));

            await newFile.DeleteAsync();
            // TODO: Cdp needs to support deleting empty folders.
        }
    }
}
