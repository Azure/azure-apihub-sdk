using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    internal class LocalFolderItem : IFolderItem
    {
        internal string _path;
        internal string _rootPath;

        public Task<string> HandleId
        {
            get
            {
                return Task.FromResult(System.IO.Path.GetDirectoryName(_path));
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public IFileItem GetFileReference(string path, bool overwrite = true)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            path = System.IO.Path.Combine(_path, path);

            return new LocalFileItem
            {
                _path = path,
                _overwrite = overwrite,
                _rootPath = this._rootPath
            };
        }

        public IFolderItem GetFolderReference(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            path = System.IO.Path.Combine(_path, path);

            return new LocalFolderItem
            {
                _path = path,
                _rootPath = this._rootPath
            };
        }

        public IFileWatcher CreateFileWatcher(FileWatcherType fileWatcherType, Func<IFileItem, object, Task> callback, object nextItem = null, int pollIntervalInSeconds = CdpConstants.DefaultFileWatcherIntervalInSeconds)
        {
            if (callback == null)
            {
                throw new ArgumentException("Callback can not be null.");
            }

            if(!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            var poll = new LocalPoll
            {
                _callback = callback,
                _rootPath = this._rootPath
            };

            poll.Run(_path, fileWatcherType);
            
            return poll;
        }

        public Task<FileTriggerInfo> CheckForFileAsync(FileWatcherType fileWatcherType, object nextItem = null)
        {
            // TODO: Implement this later for local files based on last modified date/time
            var fileTriggerInfo = new FileTriggerInfo();

            return Task.FromResult(fileTriggerInfo);
        }

        public Task<IFileItem> GetFileItemAsync(string path)
        {
            path = System.IO.Path.Combine(_path, path);

            if(!File.Exists(path))
            {
                return Task.FromResult<IFileItem>(null);
            }

            var fileItem = new LocalFileItem
            {
                _path = path,
                _rootPath = this._rootPath
            };

            return Task.FromResult<IFileItem>(fileItem);
        }

        public Task<IFolderItem> GetFolderItemAsync(string path)
        {
            path = System.IO.Path.Combine(_path, path);

            if (!Directory.Exists(path))
            {
                return Task.FromResult<IFolderItem>(null);
            }

            var folderItem = new LocalFolderItem
            {
                _path = path,
                _rootPath = this._rootPath
            };

            return Task.FromResult<IFolderItem>(folderItem);
        }

        public Task<MetadataInfo> GetMetadataAsync()
        {
            var metaData = new MetadataInfo
            {
                Path = _path,
                DisplayName = _path,
                Id = System.IO.Path.GetDirectoryName(_path),
                IsFolder = true,
                LastModified = Directory.GetLastWriteTimeUtc(_path),
                Name = System.IO.Path.GetDirectoryName(_path)
            };

            return Task.FromResult(metaData);
        }

        public Task<IItem[]> ListAsync(bool includeSubdirectories = false)
        {
            List<IItem> items = new List<IItem>();

            if(!Directory.Exists(_path))
            {
                return Task.FromResult(items.ToArray());
            }

            var files = Directory.GetFiles(_path, "*", includeSubdirectories? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach(var file in files)
            {
                items.Add(new LocalFileItem
                {
                    _path = file,
                    _rootPath = this._rootPath
                });
            }

            var folders = Directory.GetDirectories(_path, "*", includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (var folder in folders)
            {
                items.Add(new LocalFolderItem
                {
                    _path = folder,
                    _rootPath = this._rootPath
                });
            }

            return Task.FromResult(items.ToArray());
        }
    }
}
