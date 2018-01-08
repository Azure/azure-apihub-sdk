// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    /// <summary>
    /// Defines a table of entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type. Must be JObject or POCO.</typeparam>
    public interface ITable<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Gets the data set name.
        /// </summary>
        string DataSetName { get; }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Gets the table display name.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Retrieves table metadata.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The table metadata.</returns>
        Task<TableMetadata> GetMetadataAsync(
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the entity with the specified identifier.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The entity or null if not found.</returns>
        Task<TEntity> GetEntityAsync(
            string entityId,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Queries the table for entities.
        /// </summary>
        /// <param name="query">The query to be executed.</param>
        /// <param name="continuationToken">A continuation token from the server 
        /// when the operation returns a partial result.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The retrieved entities. In case of partial result the
        /// object returned will have a continuation token.</returns>
        Task<SegmentedResult<TEntity>> ListEntitiesAsync(
            Query query = null,
            ContinuationToken continuationToken = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a new entity to the table.
        /// </summary>
        /// <param name="entity">The entity to be created.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task CreateEntityAsync(
            TEntity entity,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="entity">The updated entity.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task UpdateEntityAsync(
            string entityId, 
            TEntity entity,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes an existing entity.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns></returns>
        Task DeleteEntityAsync(
            string entityId,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
