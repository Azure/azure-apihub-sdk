using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    internal class FolderItem : IFolderItem
    {
        internal string _handleId;

        internal CdpHelper _cdpHelper;

        internal string _path;

        public Task<string> HandleId
        {
            get
            {
                if (string.IsNullOrEmpty(_handleId))
                {
                    var result = GetMetadataAsync().Result;

                    if (result != null)
                    {
                        _handleId = result.Id;
                    }
                }
                return Task.FromResult(_handleId);
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public Task<IFileItem> CreateFileAsync(string path, bool overwrite = true)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return Task.FromResult<IFileItem>(null);
            }

            path = AppendToPath(path);

            return Task.FromResult<IFileItem>(            
                new FileItem
                {
                    _path = path,
                    _overwrite = overwrite,
                    _cdpHelper = this._cdpHelper
                });
        }

        public Task<IFolderItem> CreateFolderAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return Task.FromResult<IFolderItem>(null);
            }

            path = AppendToPath(path);

            return Task.FromResult<IFolderItem>(
                new FolderItem
                {
                    _path = path,
                    _cdpHelper = this._cdpHelper
                });
        }

        public IFileWatcher CreateNewFileWatcher(Func<IFileItem, Task> callback, int pollIntervalInSeconds = 30)
        {
            if(callback == null)
            {
                throw new ArgumentException("Callback can not be null.");
            }

            var poll = new Poll
            {
                _pollIntervalInSeconds = pollIntervalInSeconds,
                _cdpHelper = this._cdpHelper,
                _callback = callback
            };

            if(string.IsNullOrEmpty(_handleId))
            {
                _handleId = GetHandleIdFromPathAsync(_cdpHelper.MakeUri(CdpConstants.TopMostFolderRoot), _path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries), 0, null).GetAwaiter().GetResult();
            }

            Uri firstUrl = _cdpHelper.MakeUri(CdpConstants.OnNewFileTemplate, _handleId);

            Task tIgnore = poll.Run(firstUrl);
            poll._runTask = tIgnore;

            return poll;
        }

        public IFileWatcher CreateUpdateFileWatcher(Func<IFileItem, Task> callback, int pollIntervalInSeconds = 30)
        {
            if (callback == null)
            {
                throw new ArgumentException("Callback can not be null.");
            }

            var poll = new Poll
            {
                _pollIntervalInSeconds = pollIntervalInSeconds,
                _cdpHelper = this._cdpHelper,
                _callback = callback
            };

            if (string.IsNullOrEmpty(_handleId))
            {
                _handleId = GetHandleIdFromPathAsync(_cdpHelper.MakeUri(CdpConstants.TopMostFolderRoot), _path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries), 0, null).GetAwaiter().GetResult();
            }

            Uri firstUrl = _cdpHelper.MakeUri(CdpConstants.OnUpdateFileTemplate, _handleId);

            Task tIgnore = poll.Run(firstUrl);
            poll._runTask = tIgnore;

            return poll;
        }

        public Task<MetadataInfo> GetMetadataAsync()
        {
            //metadata not supported for folders
            return null;
        }

        public async Task<IItem[]> ListAsync(bool includeSubdirectories = false)
        {
            Uri uri = null;

            // if path is null we need to start from the root. 
            if (string.IsNullOrEmpty(this._path))
            {
                uri = _cdpHelper.MakeUri(CdpConstants.TopMostFolderRoot);
            }
            else
            {
                // if handleId is null we need to start from the root and work our way down to get the current handleId. 
                if (string.IsNullOrEmpty(_handleId))
                {
                    string[] segments = null;

                    if (!string.IsNullOrEmpty(_path))
                    {
                        segments = _path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    _handleId = await GetHandleIdFromPathAsync(_cdpHelper.MakeUri(CdpConstants.TopMostFolderRoot), segments, 0, null);

                    // if _handleId is still null, it means the folder doesn't exist and we just return an empty array same as Directory.GetFiles() method.
                    if (string.IsNullOrEmpty(_handleId))
                    {
                        return new FolderItem[0];
                    }
                }

                uri = _cdpHelper.MakeUri(CdpConstants.FoldersRoot + "/" + _handleId);
            }

            List<IItem> items = new List<IItem>();
            var itemsArray = await _cdpHelper.SendResultAsync<MetadataInfo[]>(HttpMethod.Get, uri);

            //TODO: check how many nested folders can be supported to avoid stackoverflow.
            if (itemsArray != null && includeSubdirectories)
            {
                List<MetadataInfo> nestedItems = new List<MetadataInfo>();

                nestedItems.AddRange(itemsArray);
                foreach (var nestedItem in nestedItems)
                {
                    if (nestedItem.IsFolder)
                    {
                        items.Add(new FolderItem
                        {
                            _cdpHelper = this._cdpHelper,
                            _path = nestedItem.Path,
                            _handleId = nestedItem.Id
                        });

                        var nestedFolder = await this.GetFolderItemAsync(nestedItem.Path, nestedItem.Id, _cdpHelper.RuntimeEndpoint, _cdpHelper.AccessTokenScheme, _cdpHelper.AccessToken);

                        var items2 = await nestedFolder.ListAsync(includeSubdirectories);

                        items.AddRange(items2);
                    }
                    else
                    {
                        items.Add(new FileItem
                        {
                            _cdpHelper = this._cdpHelper,
                            _path = nestedItem.Path,
                            _handleId = nestedItem.Id
                        });
                    }
                }
                return items.ToArray();
            }

            foreach (var item in itemsArray)
            {
                if (item.IsFolder)
                {
                    items.Add(new FolderItem
                    {
                        _cdpHelper = this._cdpHelper,
                        _path = item.Path,
                        _handleId = item.Id
                    });
                }
                else
                {
                    items.Add(new FileItem
                    {
                        _cdpHelper = this._cdpHelper,
                        _path = item.Path,
                        _handleId = item.Id
                    });
                }
            }

            return items.ToArray();
        }

        public async Task<IFileItem> GetFileItemAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            IFileItem fileItem = await CreateFileAsync(path);

            var result = await fileItem.GetMetadataAsync();

            if (result == null)
            {
                return null;
            }

            return fileItem;
        }

        public async Task<IFolderItem> GetFolderItemAsync(string path)
        {
            // let's first get the handleId of the current folder if it's null.
            if (string.IsNullOrEmpty(_handleId))
            {
                string[] segments = null;

                if (!string.IsNullOrEmpty(_path))
                {
                    segments = _path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                }

                _handleId = await GetHandleIdFromPathAsync(_cdpHelper.MakeUri(CdpConstants.TopMostFolderRoot), segments, 0, null);

                if (string.IsNullOrEmpty(_handleId))
                {
                    return null;
                }
            }

            string[] parts = null;

            if (!string.IsNullOrWhiteSpace(path))
            {
                parts = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (parts !=  null && parts.Length > 0)
            {
                var uri = _cdpHelper.MakeUri(CdpConstants.FoldersRoot + "/" + _handleId);

                // now let's get the handle of the folder with the given relative path
                string handleId = await GetHandleIdFromPathAsync(uri, parts, 0, null);

                if (!string.IsNullOrEmpty(handleId))
                {
                    return new FolderItem
                    {
                        _cdpHelper = this._cdpHelper,
                        _handleId = handleId,
                        _path = AppendToPath(path),
                    };
                }
            }
            else
            {
                // we are asking a reference to the current FolderItem
                return this;
            }

            return null;
        }

        private string AppendToPath(string path)
        {
            //TODO: clean this up.
            if (!string.IsNullOrEmpty(path) && !path.StartsWith("/") && !string.IsNullOrEmpty(this._path) && !this._path.EndsWith("/"))
            {
                path = "/" + path;
            }
            path = string.IsNullOrEmpty(this._path) ? path : this._path + path;

            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace("//", "/");
            }

            return path;
        }

        private async Task<string> GetHandleIdFromPathAsync(Uri uri, string[] parts, int idx, MetadataInfo current)
        {
            if (idx >= parts.Length)
            {
                return current.Id;
            }

            var itemsArray = await _cdpHelper.SendResultAsync<MetadataInfo[]>(HttpMethod.Get, uri);

            foreach (var item in itemsArray)
            {
                if (item.IsFolder)
                {
                    if (item.Path == "/")
                    {
                        // This is the root folder
                        return await GetHandleIdFromPathAsync(_cdpHelper.MakeUri(CdpConstants.FoldersRoot + "/" + item.Id), parts, idx, item);
                    }
                    else if (item.Name.Equals(parts[idx], StringComparison.OrdinalIgnoreCase))
                    {
                        return await GetHandleIdFromPathAsync(_cdpHelper.MakeUri(CdpConstants.FoldersRoot + "/" + item.Id), parts, ++idx, item);
                    }
                }
            }

            return null;
        }
    }
}
