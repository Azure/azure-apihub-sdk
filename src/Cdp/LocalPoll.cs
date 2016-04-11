using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    internal enum LocalFileWatcherType
    {
        FileCreated,
        FileUpdate
    }

    internal class LocalPoll : IFileWatcher
    {
        internal Func<IFileItem, Task> _callback;
        internal string _rootPath;

        private FileSystemWatcher _watcher;

        public async Task StopAsync()
        {
            if(_watcher != null && _watcher.EnableRaisingEvents)
            {
                _watcher.EnableRaisingEvents = false;
            }

            await Task.FromResult(0);
        }

        public void Run(string path, LocalFileWatcherType watcherType)
        {
            _watcher = new FileSystemWatcher(path);

            if (watcherType == LocalFileWatcherType.FileCreated)
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

            _callback(localFileItem);
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            // relative path is needed here.
            var path = e.FullPath.Replace(_rootPath, string.Empty);

            if(path.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Remove(0, 1);
            }

            path = path.Replace("\\", "/");

            var localFileItem = new LocalFileItem
            {
                _path = path,
                _rootPath = this._rootPath,
            };

            _callback(localFileItem);
        }
    }
}
