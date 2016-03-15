using System;

namespace Microsoft.Azure.ApiHub
{
    // Raw file protocol from CDP
    // File or Folder
    public class MetadataInfo 
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public DateTime LastModified { get; set; }

        public bool IsFolder { get; set; }

        public string Etag { get; set; }

        public string FileLocator { get; set; }

        public string MediaType { get; set; }
    }
}