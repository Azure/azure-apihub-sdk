using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub.Sdk.Table
{
    public class TableMetadata
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string Permission { get; set; }

        public TableCapabilitiesMetadata Capabilities { get; set; }

        public JObject Schema { get; set; }
    }
}
