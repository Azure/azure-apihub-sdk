// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    internal class LocalPoll : IFileWatcher
    {
        internal Func<IFileItem, object, Task> _callback;
        internal string _rootPath;

        private FileSystemWatcher _watcher;

        public async Task StopAsync()
        {
            if (_watcher != null && _watcher.EnableRaisingEvents)
            {
                _watcher.EnableRaisingEvents = false;
            }

            await Task.FromResult(0);
        }

        public void Run(string path, FileWatcherType watcherType)
        {
            _watcher = new FileSystemWatcher(path);

            if (watcherType == FileWatcherType.Created)
            {
                _watcher.Created += FileCreated;
            }
            else
            {
                _watcher.Changed += FileChanged;
            }

            _watcher.EnableRaisingEvents = true;
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            // relative path is needed here.
            var path = e.FullPath.Replace(_rootPath, string.Empty);

            if (path.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Remove(0, 1);
            }

            path = path.Replace("\\", "/");

            var localFileItem = new LocalFileItem
            {
                _path = path,
                _rootPath = this._rootPath,
            };

            _callback(localFileItem, DateTime.Now);
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            // relative path is needed here.
            var path = e.FullPath.Replace(_rootPath, string.Empty);

            if (path.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Remove(0, 1);
            }

            path = path.Replace("\\", "/");

            var localFileItem = new LocalFileItem
            {
                _path = path,
                _rootPath = this._rootPath,
            };

            _callback(localFileItem, DateTime.Now);
        }
    }
}
