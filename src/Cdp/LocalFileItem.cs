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
        internal string _rootPath;

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
            string newPath = GetUpdatedPath();

            if (!File.Exists(newPath))
            {
                return Task.FromResult(0);
            }

            File.Delete(newPath);
            return Task.FromResult(0);
        }

        public Task<MetadataInfo> GetMetadataAsync()
        {
            string newPath = GetUpdatedPath();

            if (!File.Exists(newPath))
            {
                return Task.FromResult<MetadataInfo>(null);
            }

            var metaData = new MetadataInfo
            {
                Path = newPath,
                DisplayName = newPath,
                Id = System.IO.Path.GetFileName(newPath),
                IsFolder = false,
                LastModified = File.GetLastWriteTimeUtc(newPath),
                Name = System.IO.Path.GetFileName(newPath),
                Size = File.ReadAllBytes(newPath).Length
            };

            return Task.FromResult(metaData);
        }

        public Task<byte[]> ReadAsync()
        {
            string newPath = GetUpdatedPath();

            if (!File.Exists(newPath))
            {
                throw new FileNotFoundException(newPath);
            }

            return Task.FromResult(File.ReadAllBytes(newPath));
        }

        public Task WriteAsync(byte[] contents)
        {
            string newPath = GetUpdatedPath();

            if (File.Exists(newPath))
            {
                // TODO decide what we should do here.
                if (!_overwrite)
                {
                    return Task.FromResult(0);
                }
            }
            else
            {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(newPath)))
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(newPath));
                }
            }

            if (contents != null)
            {
                using (var fs = File.OpenWrite(newPath))
                {
                    fs.Write(contents, 0, contents.Length);
                    fs.Flush();
                }
            }
            else
            {
                File.WriteAllBytes(newPath, new byte[] { });
            }

            return Task.FromResult(0);
        }

        private string GetUpdatedPath()
        {
            if (!_path.StartsWith(_rootPath, StringComparison.OrdinalIgnoreCase))
            {
                return (_rootPath + _path);
            }
            else
            {
                return _path;
            }
        }
    }
}
