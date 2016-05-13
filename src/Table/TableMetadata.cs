using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub.Sdk.Table
{
    public class TableMetadata
    {
        internal TableMetadata()
        {            
        }

        public string Name { get; internal set; }

        public string Title { get; internal set; }

        public string Permission { get; internal set; }

        public TableCapabilitiesMetadata Capabilities { get; internal set; }

        public JObject Schema { get; internal set; }
    }
}
