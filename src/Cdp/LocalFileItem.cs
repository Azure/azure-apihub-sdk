using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    internal class LocalFileItem : IFileItem
    {
        internal string _path;

        internal bool _overwrite;

        public Task<string> HandleId
        {
            get
            {
                return Task.FromResult(System.IO.Path.GetFileName(_path));
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public Task DeleteAsync()
        {
            if (!File.Exists(_path))
            {
                return Task.FromResult(0);
            }

            File.Delete(_path);
            return Task.FromResult(0);
        }

        public Task<MetadataInfo> GetMetadataAsync()
        {
            if (!File.Exists(_path))
            {
                return Task.FromResult<MetadataInfo>(null);
            }

            var metaData = new MetadataInfo
            {
                Path = _path,
                DisplayName = _path,
                Id = System.IO.Path.GetFileName(_path),
                IsFolder = false,
                LastModified = File.GetLastWriteTimeUtc(_path),
                Name = System.IO.Path.GetFileName(_path)
            };

            return Task.FromResult(metaData);
        }

        public Task<byte[]> ReadAsync()
        {
            if(!File.Exists(_path))
            {
                throw new FileNotFoundException(_path);
            }

            return Task.FromResult(File.ReadAllBytes(_path));
        }

        public Task WriteAsync(byte[] contents)
        {
            if(File.Exists(_path))
            {
                // TODO decide what we should do here.
                if (!_overwrite)
                {
                    return Task.FromResult(0);
                }
            }
            else
            {
                _path = _path.Replace('/', '\\');

                if(!Directory.Exists(System.IO.Path.GetDirectoryName(_path)))
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_path));
                }

                using (var fs = File.Create(_path))
                {
                    fs.Flush();
                }
            }

            if (contents != null)
            {
                File.WriteAllBytes(_path, contents);
            }

            return Task.FromResult(0);
        }
    }
}
