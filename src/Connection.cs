using System;
using Microsoft.Azure.ApiHub.Common;
using Microsoft.Azure.ApiHub.Table.Internal;

namespace Microsoft.Azure.ApiHub
{
    /// <summary>
    /// Represents a connection to an ApiHub connector.
    /// </summary>
    public class Connection
    {
        private ConnectionSettings ConnectionSettings { get; set; }

        /// <summary>
        /// Purposed only for testing.
        /// </summary>
        protected Connection()
        {
        }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public Connection(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentException("The connection string must not be null or empty", "connectionString");
            }

            ConnectionSettings = ConnectionSettings.Parse(connectionString);
        }

        /// <summary>
        /// Creates a client with tabular capabilities.
        /// </summary>
        /// <returns>The table client.</returns>
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
