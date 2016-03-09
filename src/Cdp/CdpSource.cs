using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microfoft.Azure.ApiHub.Sdk.Cdp
{
    public class CdpSource : IFileSource
    {
        private CdpHelper _cdpHelper;

        public static string GetConnectionString(Uri runtimeEndpoint, string scheme, string accessToken)
        {
            return string.Format(CdpConstants.CdpConnectionStringTemplate, runtimeEndpoint, scheme, accessToken);
        }

        public static async Task<CdpSource> ParseAsync(string connectionString)
        {
            Uri runtimeEndpoint;
            string accessTokenScheme;
            string accessToken;

            ParseConnectionString(connectionString, out runtimeEndpoint, out accessTokenScheme, out accessToken);

            var source = New(runtimeEndpoint, accessToken, accessTokenScheme);

            var metadata = await source.GetMetadataAsync();

            if (!metadata.HasFileSupport)
            {
                throw new InvalidOperationException("File-access not supported for this connection");
            }

            return source;
        }

        private static void ParseConnectionString(string connectionString, out Uri runtimeEndpoint, out string accessTokenScheme, out string accessToken)
        {
            //TODO: this needs more clean up.
            runtimeEndpoint = null;
            accessTokenScheme = null;
            accessToken = null;

            var parts = connectionString.Split(';');

            if(parts.Length != 3)
            {
                throw new FormatException("Invalid connectionstring: " + connectionString );
            }

            foreach(var part in parts)
            {
                var keyValue = part.Split('=');

                if(keyValue.Length != 2)
                {
                    throw new FormatException("Invalid connectionstring: " + connectionString);
                }

                var key = keyValue[0];
                var value = keyValue[1];

                switch(key.ToLowerInvariant())
                {
                    case "endpoint":
                        runtimeEndpoint = new Uri(value);
                        break;
                    case "scheme":
                        accessTokenScheme = value;
                        break;
                    case "accesstoken":
                        accessToken = value;
                        break;
                    default:
                        throw new FormatException("Invalid connectionstring: " + connectionString);
                }
            }
        }

        #region IFileSource implemenentation
        public async Task<CdpItemInfo> GetIdAsync(string path)
        {
            var uri = _cdpHelper.MakeUri(CdpConstants.FileMetadataByPathTemplate, path);

            HttpResponseMessage response = await _cdpHelper.SendAsync(HttpMethod.Get, uri);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            var result = await _cdpHelper.DecodeAsync<CdpItemInfo>(response);

            return result;
        }

        public async Task<CdpItemInfo> CreateAsync(string path, byte[] contents, bool overwrite = true)
        {
            if (overwrite)
            {
                // Check if the file exists, in which case we need to use update
                var id = await GetIdAsync(path);
                if (id != null)
                {                    
                    return await UpdateAsync(id, contents);
                }
            }

            string folder = Path.GetDirectoryName(path);
            string name = Path.GetFileName(path);

            var uri = _cdpHelper.MakeUri(CdpConstants.CreateFileTemplate, folder, name);

            ByteArrayContent content = new ByteArrayContent(contents);
            var result = await _cdpHelper.SendResultAsync<CdpItemInfo>(HttpMethod.Post, uri, content);

            return result;
        }

        public async Task<byte[]> ReadAsync(string path)
        {
            var uri = _cdpHelper.MakeUri(CdpConstants.FileContentByPathTemplate, path);

            return await _cdpHelper.SendRawAsync(HttpMethod.Get, uri);
        }

        public async Task<byte[]> ReadAsync(CdpItemInfoBase item)
        {
            var uri = _cdpHelper.MakeUri(CdpConstants.FileContentByIdTemplate, item.Id);

            return await _cdpHelper.SendRawAsync(HttpMethod.Get, uri);
        }

        public async Task<CdpItemInfo> UpdateAsync(CdpItemInfoBase item, byte[] contents)
        {
            var uri = _cdpHelper.MakeUri(CdpConstants.FileMetadataByIdTemplate, item.Id);

            ByteArrayContent content = new ByteArrayContent(contents);

            HttpResponseMessage response = await _cdpHelper.SendAsync(HttpMethod.Put, uri, content);

            var result = await _cdpHelper.DecodeAsync<CdpItemInfo>(response);

            return result;
        }

        public async Task DeleteAsync(CdpItemInfoBase item)
        {
            var uri = _cdpHelper.MakeUri(CdpConstants.FileMetadataByIdTemplate, item.Id);

            HttpResponseMessage response = await _cdpHelper.SendAsync(HttpMethod.Delete, uri);
        }

        public async Task<CdpItemInfo> GetMetaDataAsync(string path)
        {
            var uri = _cdpHelper.MakeUri(CdpConstants.FileMetadataByPathTemplate, path);

            HttpResponseMessage response = await _cdpHelper.SendAsync(HttpMethod.Get, uri);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var result = await _cdpHelper.DecodeAsync<CdpItemInfo>(response);

            return result;
        }

        public async Task<CdpItemInfo> GetMetaDataAsync(CdpItemInfoBase item)
        {
            var uri = _cdpHelper.MakeUri(CdpConstants.FileMetadataByIdTemplate, item.Id);

            HttpResponseMessage response = await _cdpHelper.SendAsync(HttpMethod.Get, uri);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var result = await _cdpHelper.DecodeAsync<CdpItemInfo>(response);

            return result;
        }

        public async Task<CdpItemInfo[]> ListAsync(CdpItemInfoBase item, bool includeSubdirectories = false)
        {
            Uri uri = null;
            if (item == null)
            {
                uri = _cdpHelper.MakeUri(CdpConstants.RootFolders);
            }
            else
            {
                uri = _cdpHelper.MakeUri(CdpConstants.FoldersRoot + "/" + item.Id);
            }

            var itemsArray = await _cdpHelper.SendResultAsync<CdpItemInfo[]>(HttpMethod.Get, uri);

            //TODO: check how many nested folders can be supported to avoid stackoverflow.
            if (includeSubdirectories)
            {
                List<CdpItemInfo> items = new List<CdpItemInfo>();

                items.AddRange(itemsArray);
                foreach (var cdpItem in itemsArray)
                {
                    if(cdpItem.IsFolder)
                    {
                        var items2 = await ListAsync(cdpItem, includeSubdirectories);
                        items.AddRange(items2);
                    }
                }
                return items.ToArray();
            }

            return itemsArray;
        }

        public IFileWatcher CreateNewFileWatcher(string folder, Func<CdpItemInfo, Task> callback, int pollIntervalInSeconds = CdpConstants.DefaultFileWatcherIntervalInSeconds)
        {
            var poll = new Poll
            {
                _pollIntervalInSeconds = pollIntervalInSeconds,
                _cdpHelper = this._cdpHelper,
                _callback = callback
            };

            Uri firstUrl = _cdpHelper.MakeUri(CdpConstants.OnNewFileTemplate, folder);

            Task tIgnore = poll.Run(firstUrl);
            poll._runTask = tIgnore;

            return poll;
        }

        public IFileWatcher CreateUpdateFileWatcher(string folder, Func<CdpItemInfo, Task> callback, int pollIntervalInSeconds = CdpConstants.DefaultFileWatcherIntervalInSeconds)
        {
            var poll = new Poll
            {
                _pollIntervalInSeconds = pollIntervalInSeconds,
                _cdpHelper = this._cdpHelper,
                _callback = callback
            };

            Uri firstUrl = _cdpHelper.MakeUri(CdpConstants.OnUpdateFileTemplate, folder);

            Task tIgnore = poll.Run(firstUrl);
            poll._runTask = tIgnore;

            return poll;
        }

        #endregion

        private async Task<MetadataResponse> GetMetadataAsync()
        {
            var uri = _cdpHelper.MakeUri(CdpConstants.DatasetMetadata);

            var result = await _cdpHelper.SendResultAsync<MetadataResponse>(HttpMethod.Get, uri);

            return result;
        }

        private static CdpSource New(Uri runtimeEndpoint, string accessToken, string scheme)
        {
            return new CdpSource
            {
                _cdpHelper = new CdpHelper(runtimeEndpoint, scheme, accessToken)
            };
        }
    }
}
