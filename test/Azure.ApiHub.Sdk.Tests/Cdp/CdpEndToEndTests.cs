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
            string cdpTestRoot = "/tests/" + Guid.NewGuid().ToString();
            string fileName = Guid.NewGuid().ToString();

            var folder = _rootFolder.GetFolderReference(cdpTestRoot);
            var file = folder.GetFileReference(fileName);
            await file.WriteAsync(new byte[] { });

            IItem[] currentList = await folder.ListAsync(false);
            Assert.Single(currentList);

            currentList = await folder.ListAsync(true);
            Assert.Single(currentList);

            var nestedFolder = folder.GetFolderReference("nestedFolder");
            var file2 = nestedFolder.GetFileReference(fileName);
            await file2.WriteAsync(new byte[] { });

            currentList = await nestedFolder.ListAsync(true);
            Assert.Single(currentList);

            currentList = await folder.ListAsync(true);
            Assert.Equal(3, currentList.Length);

            await file2.DeleteAsync();

            currentList = await folder.ListAsync(true);
            Assert.Equal(2, currentList.Length);

            await file.DeleteAsync();

            currentList = await folder.ListAsync(true);
            Assert.Single(currentList);
        }

        [Fact]
        public async Task FileCrudTestsAsync()
        {
            string cdpTestRoot = "tests/" + Guid.NewGuid().ToString();
            string fileName = Guid.NewGuid().ToString();

            var folder = _rootFolder.GetFolderReference(cdpTestRoot);

            var file = folder.GetFileReference(fileName, true);

            var content = "test\r\n";
            await file.WriteAsync(Encoding.Default.GetBytes(content));


            Assert.Contains(fileName, file.Path);
            Assert.Contains(cdpTestRoot, file.Path);

            var retrievedContent = Encoding.Default.GetString(await file.ReadAsync());

            Assert.Equal(content, retrievedContent);

            var metadata = await file.GetMetadataAsync();

            Assert.False(metadata.IsFolder);

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

            var folder = _rootFolder.GetFolderReference(cdpTestRoot);

            // Create an empty file to create an empty new folder on SAAS provider
            var initialFile = folder.GetFileReference(Guid.NewGuid().ToString());

            await initialFile.WriteAsync(new byte[0]);

            string fileName = Guid.NewGuid().ToString();

            var poll = folder.CreateFileWatcher(FileWatcherType.Created,
                (fr, obj) =>
                {
                    Assert.Contains(fileName, fr.Path);

                    var uri = obj as Uri;
                    if (uri != null)
                    {
                        Assert.True(!string.IsNullOrEmpty(uri.AbsolutePath));
                    }

                    fileAddedTrigger = true;

                    return Task.FromResult(0);
                }, null, 1);

            var poll2 = folder.CreateFileWatcher(FileWatcherType.Updated,
                (fr, obj) =>
                {
                    Assert.Contains(fileName, fr.Path);

                    var uri = obj as Uri;
                    if (uri != null)
                    {
                        Assert.True(!string.IsNullOrEmpty(uri.AbsolutePath));
                    }

                    fileUpdatedTrigger = true;
                    return Task.FromResult(0);
                }, null, 1);

            var newFile = folder.GetFileReference(fileName);
            await newFile.WriteAsync(new byte[0]);

            // Adding some wait to make sure triggers are fired.
            await Task.Delay(15000);

            await newFile.WriteAsync(new byte[1] { 0 });

            // Adding some wait to make sure triggers are fired.
            await Task.Delay(15000);

            Assert.True(fileAddedTrigger);
            Assert.True(fileUpdatedTrigger);

            var triggerType = await folder.CheckForFileAsync(FileWatcherType.Created);

            var fileName2 = Guid.NewGuid().ToString();
            var newFile2 = folder.GetFileReference(fileName2);
            await newFile2.WriteAsync(new byte[0]);

            triggerType = await folder.CheckForFileAsync(FileWatcherType.Created, triggerType.NextUri);

            if (triggerType.FileItem != null)
            {
                Assert.Equal(newFile2.Path, triggerType.FileItem.Path, ignoreCase: true);
            }

            await newFile.DeleteAsync();
            await newFile2.DeleteAsync();
            await initialFile.DeleteAsync();
        }

        [Fact]
        public async Task NonExistenceTestsAsync()
        {
            string cdpTestRoot = "tests/" + Guid.NewGuid().ToString();
            string fileName = Guid.NewGuid().ToString();

            var folder = _rootFolder.GetFolderReference(cdpTestRoot);

            // file doesn't exist test 
            var fileItem = await folder.GetFileItemAsync(Guid.NewGuid().ToString());

            Assert.Null(fileItem);

            IFolderItem nullFolder = folder.GetFolderReference(string.Empty);

            Assert.Null(nullFolder);

            var nullFile = folder.GetFileReference(string.Empty);

            Assert.Null(nullFile);

            var newFolder = folder.GetFolderReference(Guid.NewGuid().ToString());

            // listing items for a folder which doesn't exist
            var listItems = await newFolder.ListAsync();

            Assert.Empty(listItems);

            var newFile = newFolder.GetFileReference(Guid.NewGuid().ToString());

            try
            {
                // trying to read from a file which doesn't exist should throw an exception.
                var bytes = await newFile.ReadAsync();
            }
            catch (Exception ex)
            {
                // Expected exception was thrown when attempting to read from a file which doesn't exist.
                Assert.IsType<FileNotFoundException>(ex);
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
            Assert.Equal(0, metadata.Size);

            await newFile.WriteAsync(new byte[] { 0 });
            metadata = await newFile.GetMetadataAsync();

            Assert.Equal(1, metadata.Size);

            string newFolderName = Guid.NewGuid().ToString();
            string newFileName = Guid.NewGuid().ToString();

            Assert.False(await folder.FolderExistsAsync(newFolderName));
            Assert.False(await folder.FileExistsAsync(newFileName));

            newFile = folder.GetFileReference(newFolderName + "/" + newFileName);
            await newFile.WriteAsync(null);

            Assert.True(await folder.FolderExistsAsync(newFolderName));
            Assert.True(await folder.FileExistsAsync(newFolderName + "/" + newFileName));

            await newFile.DeleteAsync();
            // TODO: Cdp needs to support deleting empty folders.
        }

        [Fact]
        public Task FileWatchOnNonExistenceFolderTestsAsync()
        {
            string cdpTestRoot = Guid.NewGuid().ToString();

            var folder = _rootFolder.GetFolderReference(cdpTestRoot);

            string fileName = Guid.NewGuid().ToString();

            var poll = folder.CreateFileWatcher(FileWatcherType.Created,
                (fr, obj) =>
                {
                    return Task.FromResult(0);
                });

            Assert.Null(poll);

            return Task.FromResult(0);
        }
    }
}
