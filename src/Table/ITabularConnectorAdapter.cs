using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Sdk.Common;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub.Sdk.Table
{
    /// <summary>
    /// Connector data protocol (CDP) adapter for tabular connectors.
    /// Defines the flat list of APIs provided by the connector.
    /// </summary>
    public interface ITabularConnectorAdapter
    {
        /// <summary>
        /// Queries the tabular connector for data sets.
        /// </summary>
        /// <param name="query">The query to be executed.</param>
        /// <param name="continuationToken">A continuation token from the server 
        /// when the operation returns a partial result.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The retrieved data sets. In case of partial result the
        /// object returned will have a continuation token.</returns>
        Task<SegmentedResult<IDataSet>> ListDataSetsAsync(
            Query query = null,
            ContinuationToken continuationToken = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Queries the tabular connector for tables in a data set.
        /// </summary>
        /// <param name="dataSetName">The data set name.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="continuationToken">A continuation token from the server 
        /// when the operation returns a partial result.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The retrieved entities. In case of partial result the
        /// object returned will have a continuation token.</returns>
        Task<SegmentedResult<ITable<JObject>>> ListTablesAsync(
            string dataSetName,
            Query query = null,
            ContinuationToken continuationToken = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves table metadata.
        /// </summary>
        /// <param name="dataSetName">The data set name.</param>
        /// <param name="tableName">The data set name.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The table metadata.</returns>
        Task<TableMetadata> GetTableMetadataAsync(
            string dataSetName,
            string tableName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the entity with the specified identifier.
        /// </summary>
        /// <param name="dataSetName">The data set name.</param>
        /// <param name="tableName">The data set name.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The entity or null if not found.</returns>
        Task<TEntity> GetEntityAsync<TEntity>(
            string dataSetName,
            string tableName,
            string entityId,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Queries the tabular connector for entities in a table.
        /// </summary>
        /// <param name="dataSetName">The data set name.</param>
        /// <param name="tableName">The data set name.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="continuationToken">A continuation token from the server 
        /// when the operation returns a partial result.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The retrieved entities. In case of partial result the
        /// object returned will have a continuation token.</returns>
        Task<SegmentedResult<TEntity>> ListEntitiesAsync<TEntity>(
            string dataSetName,
            string tableName,
            Query query = null,
            ContinuationToken continuationToken = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a new entity to a table.
        /// </summary>
        /// <param name="dataSetName">The data set name.</param>
        /// <param name="tableName">The data set name.</param>
        /// <param name="entity">The entity to be created.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task CreateEntityAsync<TEntity>(
            string dataSetName,
            string tableName,
            TEntity entity,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="dataSetName">The data set name.</param>
        /// <param name="tableName">The data set name.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="entity">The updated entity.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task UpdateEntityAsync<TEntity>(
            string dataSetName,
            string tableName,
            string entityId,
            TEntity entity,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes an existing entity.
        /// </summary>
        /// <param name="dataSetName">The data set name.</param>
        /// <param name="tableName">The data set name.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task DeleteEntityAsync(
            string dataSetName,
            string tableName,
            string entityId,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
