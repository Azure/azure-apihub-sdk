using Microsoft.Azure.ApiHub.Sdk.Common;
using Microsoft.Azure.ApiHub.Sdk.Tabular;
using Microsoft.Azure.ApiHub.Sdk.Tabular.Internal;

namespace Microsoft.Azure.ApiHub.Sdk
{
    public class ClientFactory
    {
        public string ConnectionString { get; private set; }

        public ClientFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public virtual ITableClient CreateTableClient()
        {
            var settings = ConnectionSettings.Parse(ConnectionString);

            // TODO: Check the settings and instantiate a local implementation if specified.

            return new TableClient(new TabularConnectorAdapter
            {
                Client = new ConnectorHttpClient(
                    settings.RuntimeEndpoint, 
                    settings.AccessTokenScheme, 
                    settings.AccessToken),
                ProtocolToModel = new ProtocolToModelConverter()
            });
        }
    }
}
