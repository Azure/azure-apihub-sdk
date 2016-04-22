using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub.Sdk.Tabular
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

        // TODO: Replace JObject with strong type. The swagger does not define the schema.
        public JObject Schema { get; internal set; }
    }
}
