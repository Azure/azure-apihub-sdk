using Microsoft.Azure.ApiHub.Sdk.Common;

namespace Microsoft.Azure.ApiHub.Sdk.Tabular.Internal
{
    internal class TabularConnectorAdapter
    {
        public ConnectorHttpClient Client { get; set; }

        public ProtocolToModelConverter ProtocolToModel { get; set; }
    }
}
