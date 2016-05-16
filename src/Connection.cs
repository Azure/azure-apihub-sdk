using Microsoft.Azure.ApiHub.Sdk.Common;
using Microsoft.Azure.ApiHub.Sdk.Table;

namespace Microsoft.Azure.ApiHub.Sdk
{
    /// <summary>
    /// Represents a connection to an ApiHub connector.
    /// </summary>
    public class Connection
    {
        private ConnectionSettings ConnectionSettings { get; set; }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public Connection(string connectionString)
        {
            ConnectionSettings = ConnectionSettings.Parse(connectionString);
        }

        /// <summary>
        /// Creates a client with tabular capabilities.
        /// </summary>
        /// <returns>The table client.</returns>
        public virtual ITableClient CreateTableClient()
        {
            return CreateTableClient(
                new TabularConnectorAdapter(
                    new ConnectorHttpClient(
                        ConnectionSettings.RuntimeEndpoint,
                        ConnectionSettings.AccessTokenScheme,
                        ConnectionSettings.AccessToken),
                    new ProtocolToModelConverter()));
        }

        /// <summary>
        /// Creates a client with tabular capabilities using the specified adapter.
        /// <param name="adapter">The tabular connector adapter.</param>
        /// </summary>
        /// <returns>The table client.</returns>
        protected ITableClient CreateTableClient(ITabularConnectorAdapter adapter)
        {
            return new TableClient(adapter);
        }
    }
}
