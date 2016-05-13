using Microsoft.Azure.ApiHub.Sdk.Common;
using Microsoft.Azure.ApiHub.Sdk.Table;

namespace Microsoft.Azure.ApiHub.Sdk
{
    public class ClientFactory
    {
        private ConnectionSettings ConnectionSettings { get; set; }

        public ClientFactory(string connectionString)
        {
            ConnectionSettings = ConnectionSettings.Parse(connectionString);
        }

        public virtual ITableClient CreateTableClient()
        {
            return new TableClient(
                new TabularConnectorAdapter(
                    new ConnectorHttpClient(
                        ConnectionSettings.RuntimeEndpoint,
                        ConnectionSettings.AccessTokenScheme,
                        ConnectionSettings.AccessToken),
                    new ProtocolToModelConverter()));
        }
    }
}
