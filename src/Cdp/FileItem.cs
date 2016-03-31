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
    internal class FileItem : IFileItem
    {
        internal string _handleId;

        internal CdpHelper _cdpHelper;

        internal string _path;

        internal bool _overwrite;

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

        public async Task DeleteAsync()
        {
            if(string.IsNullOrEmpty(_handleId))
            {
                _handleId = await HandleId;

                // if _handleId is null we make one attempt to populate it.
                if (string.IsNullOrEmpty(_handleId))
                {
                    return;
                }
            }

            var uri = _cdpHelper.MakeUri(CdpConstants.FileMetadataByIdTemplate, _handleId);
            await _cdpHelper.SendAsync(HttpMethod.Delete, uri);

            _handleId = null;
        }

        public async Task<MetadataInfo> GetMetadataAsync()
        {
            var uri = _cdpHelper.MakeUri(CdpConstants.FileMetadataByPathTemplate, _path);

            HttpResponseMessage response = await _cdpHelper.SendAsync(HttpMethod.Get, uri);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var metadata = await _cdpHelper.DecodeAsync<MetadataInfo>(response);
            _handleId = metadata.Id;

            return metadata;
        }

        public async Task<byte[]> ReadAsync()
        {
            byte[] result = null;
            Uri uri = null; 

            if (!string.IsNullOrEmpty(_handleId))
            {
                uri = _cdpHelper.MakeUri(CdpConstants.FileContentByIdTemplate, _handleId);
                result = await _cdpHelper.SendRawAsync(HttpMethod.Get, uri);
            }
            else if (_path != null)
            {
                uri = _cdpHelper.MakeUri(CdpConstants.FileContentByPathTemplate, _path);
                result = await _cdpHelper.SendRawAsync(HttpMethod.Get, uri);
            }

            if(result == null)
            {
                throw new FileNotFoundException(uri != null ? uri.PathAndQuery : string.Empty);
            }
            else
            {
                return result;
            }
        }

        public async Task WriteAsync(byte[] contents)
        {
            if (_overwrite)
            {
                if (!string.IsNullOrEmpty(_handleId))
                {
                    await UpdateAsync(contents);
                    return;
                }
            }

            string folder = System.IO.Path.GetDirectoryName(_path).Replace("\\", "/");
            string name = System.IO.Path.GetFileName(_path);

            var uri = _cdpHelper.MakeUri(CdpConstants.CreateFileTemplate, folder, name);

            var result = await _cdpHelper.SendResultAsync<MetadataInfo>(HttpMethod.Post, uri, contents);

            // TODO: what to do otherwise? throw an exception?
            if (result.Item2 == HttpStatusCode.OK)
            {
                _handleId = result.Item1.Id;
            }
            else if(result.Item2 == HttpStatusCode.Conflict)
            {
                //File already exists, trying to update.
                if(_overwrite)
                {
                    _handleId = await HandleId;

                    await UpdateAsync(contents);
                }
            }
        }

        private async Task UpdateAsync(byte[] contents)
        {
            var uri = _cdpHelper.MakeUri(CdpConstants.FileMetadataByIdTemplate, _handleId);

            var result = await _cdpHelper.SendResultAsync<MetadataInfo>(HttpMethod.Put, uri, contents);

            // TODO: it might not be needed to update _handleId since it will most likely not get updated when updating a file.
            if (result.Item2 == HttpStatusCode.OK)
            {
                _handleId = result.Item1.Id;
            }
        }
    }
}
