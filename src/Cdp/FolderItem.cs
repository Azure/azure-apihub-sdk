using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Extensions;

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

        public IFileItem GetFileReference(string path, bool overwrite = true)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            path = AppendToPath(path);

            return new FileItem
            {
                _path = path,
                _overwrite = overwrite,
                _cdpHelper = this._cdpHelper
            };
        }

        public IFolderItem GetFolderReference(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            path = AppendToPath(path);

            return new FolderItem
            {
                _path = path,
                _cdpHelper = this._cdpHelper
            };
        }

        public IFileWatcher CreateFileWatcher(FileWatcherType fileWatcherType, Func<IFileItem, object, Task> callback, object nextItem = null, int pollIntervalInSeconds = CdpConstants.DefaultFileWatcherIntervalInSeconds)
        {
            Uri pollUri = null;

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

            if (nextItem == null)
            {
                if (string.IsNullOrEmpty(_handleId))
                {
                    _handleId = GetHandleIdFromPathAsync(_cdpHelper.MakeUri(CdpConstants.TopMostFolderRoot), _path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries), 0, null).GetAwaiter().GetResult();

                    if(_handleId == null)
                    {
                        _cdpHelper.Logger.Error("Unable to get a reference to path: " + _path);
                        return null;
                    }
                }

                if (fileWatcherType == FileWatcherType.Created)
                {
                    // FTP and SFTP connectors do not currently implement onnewfile trigger but instead onupdatefile trigger handles both file creates and updates.
                    // This is a workaround to enable file create triggers for FTP and SFTP connections.
                    // TODO: This needs to be removed if/when FTP and SFTP implement separate triggers for file create and updates.
                    if (!IsFtpOrSftpApi())
                    {
                        pollUri = _cdpHelper.MakeUri(CdpConstants.OnNewFileTemplate, _handleId);
                    }
                    else
                    {
                        pollUri = _cdpHelper.MakeUri(CdpConstants.OnUpdateFileTemplate, _handleId);
                    }
                }
                else if (fileWatcherType == FileWatcherType.Updated)
                {
                    pollUri = _cdpHelper.MakeUri(CdpConstants.OnUpdateFileTemplate, _handleId);
                }
            }
            else
            {
                pollUri = nextItem as Uri;

                if (pollUri == null)
                {
                    throw new ArgumentException("Invalid type", "nextItem");
                }
            }

            if (_handleId != null || nextItem != null)
            {
                Task tIgnore = poll.Run(pollUri);

                _cdpHelper.Logger.Info(string.Format("Started monitoring folder: {0}, pollUri: {1}", _path, pollUri.AbsoluteUri));

                poll._runTask = tIgnore;

                return poll;
            }
            else
            {
                _cdpHelper.Logger.Error("Can't create a trigger on a folder which does not exist: " + _path);

                return null;
            }
        }

        public async Task<FileTriggerInfo> CheckForFileAsync(FileWatcherType fileWatcherType, object nextItem = null)
        {
            Uri pollUri = null;

            if (nextItem == null)
            {
                if (string.IsNullOrEmpty(_handleId))
                {
                    _handleId = await GetHandleIdFromPathAsync(_cdpHelper.MakeUri(CdpConstants.TopMostFolderRoot), _path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries), 0, null);
                }

                if (fileWatcherType == FileWatcherType.Created)
                {
                    // TODO: This needs to be removed if/when FTP and SFTP implement separate triggers for file create and updates.
                    if (!IsFtpOrSftpApi())
                    {
                        pollUri = _cdpHelper.MakeUri(CdpConstants.OnNewFileTemplate, _handleId);
                    }
                    else
                    {
                        pollUri = _cdpHelper.MakeUri(CdpConstants.OnUpdateFileTemplate, _handleId);
                    }
                }
                else if (fileWatcherType == FileWatcherType.Updated)
                {
                    pollUri = _cdpHelper.MakeUri(CdpConstants.OnUpdateFileTemplate, _handleId);
                }
            }
            else
            {
                pollUri = nextItem as Uri;

                if(pollUri == null)
                {
                    throw new ArgumentException("Invalid type", "nextItem");
                }
            }

            // Only the header is required and there is no need to read the entire file content which the connector returns.
            HttpResponseMessage response = await _cdpHelper.SendAsync(HttpMethod.Get, pollUri, HttpCompletionOption.ResponseHeadersRead);

            Uri nextUri = response.Headers.Location; // poll next
            TimeSpan delay = new TimeSpan(0, 0, CdpConstants.DefaultFileWatcherIntervalInSeconds);
            IFileItem fileItem = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string fileId = response.GetHeader("x-ms-file-id");
                string fullpath = response.GetHeader("x-ms-file-path");

                // Chop off leading 
                if (fullpath[0] == '/')
                {
                    fullpath = fullpath.Substring(1);
                }

                // Got a new file 
                fileItem = new FileItem
                {
                    _path = fullpath,
                    _handleId = fileId,
                };
            }
            else if (response.StatusCode == HttpStatusCode.Accepted)
            {
                var rt = response.Headers.RetryAfter;

                if (rt.Delta.HasValue)
                {
                    delay = rt.Delta.Value;
                }
            }
            else
            {
                _cdpHelper.Logger.Error("Unable to check for file, http status code: " + response.StatusCode.ToString());
            }

            return new FileTriggerInfo
            {
                FileItem = fileItem,
                NextUri = nextUri,
                WatcherType = fileWatcherType,
                RetryAfter = delay
            };
        }

        public Task<MetadataInfo> GetMetadataAsync()
        {
            //metadata not supported for folders
            return Task.FromResult<MetadataInfo>(null);
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
            var itemsArray = (await _cdpHelper.SendResultAsync<MetadataInfo[]>(HttpMethod.Get, uri)).Item1;

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

                        var nestedFolder = this.GetFolderReference(nestedItem.Path, nestedItem.Id, _cdpHelper.RuntimeEndpoint, _cdpHelper.AccessTokenScheme, _cdpHelper.AccessToken, _cdpHelper.Logger);

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

            IFileItem fileItem = GetFileReference(path);

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
            if(idx == 0 && parts.Length == 0)
            {
                // this is the top most root.
                var itemArray = (await _cdpHelper.SendResultAsync<MetadataInfo[]>(HttpMethod.Get, uri)).Item1;

                if(itemArray != null && itemArray.Length > 0)
                {
                    return itemArray[0].Id;
                }
                else
                {
                    return null;
                }
            }

            if (idx >= parts.Length)
            {
                return current.Id;
            }

            var itemsArray = (await _cdpHelper.SendResultAsync<MetadataInfo[]>(HttpMethod.Get, uri)).Item1;

            if (itemsArray != null)
            {
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
            }

            return null;
        }

        private bool IsFtpOrSftpApi()
        {
            string url = _cdpHelper.RuntimeEndpoint.AbsoluteUri;

            if (url.IndexOf("/apim/ftp", StringComparison.OrdinalIgnoreCase) > 0 || url.IndexOf("/apim/sftp", StringComparison.OrdinalIgnoreCase) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
