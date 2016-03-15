using System;

namespace Microsoft.Azure.ApiHub.Management.Entities
{
    public class ConnectionKey
    {
        public string Key { get; internal set; }

        public Uri RuntimeUri { get; internal set; }
    }
}
