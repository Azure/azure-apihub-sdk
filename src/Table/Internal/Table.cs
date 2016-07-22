using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub.Table.Internal
{
    public class Table<TEntity> : ITable<TEntity>
        where TEntity: class
    {
        public string DataSetName { get; private set; }

        public string TableName { get; private set; }

        public string DisplayName { get; internal set; }

        internal ITabularConnectorAdapter Adapter { get; private set; }

        public Table(string dataSetName, string tableName, ITabularConnectorAdapter adapter)
        {
            if (string.IsNullOrEmpty(dataSetName))
            {
                throw new ArgumentException("The dataset name must not be null or empty.", "dataSetName");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("The table name must not be null or empty.", "tableName");
            }

            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }

            DataSetName = dataSetName;
            TableName = tableName;
            Adapter = adapter;
        }

        public async Task<TableMetadata> GetMetadataAsync(
            CancellationToken cancellationToken)
        {
            return await Adapter.GetTableMetadataAsync(
                DataSetName, 
                TableName,
                cancellationToken);
        }

        public async Task<TEntity> GetEntityAsync(
            string entityId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                throw new ArgumentException("The entity identifier must not be null or empty.", "entityId");
            }

            return await Adapter.GetEntityAsync<TEntity>(
                DataSetName, 
                TableName, 
                entityId,
                cancellationToken);
        }

        public async Task<SegmentedResult<TEntity>> ListEntitiesAsync(
            Query query,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            return await Adapter.ListEntitiesAsync<TEntity>(
                DataSetName, 
                TableName, 
                query,
                continuationToken,
                cancellationToken);
        }

        public async Task CreateEntityAsync(
            TEntity entity,
            CancellationToken cancellationToken)
        {
            await Adapter.CreateEntityAsync(
                DataSetName, 
                TableName, 
                entity,
                cancellationToken);
        }

        public async Task UpdateEntityAsync(
            string entityId, 
            TEntity entity,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                throw new ArgumentException("The entity identifier must not be null or empty.", "entityId");
            }

            await Adapter.UpdateEntityAsync(
                DataSetName, 
                TableName, 
                entityId, 
                entity,
                cancellationToken);
        }

        public async Task DeleteEntityAsync(
            string entityId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                throw new ArgumentException("The entity identifier must not be null or empty.", "entityId");
            }

            await Adapter.DeleteEntityAsync(
                DataSetName, 
                TableName, 
                entityId,
                cancellationToken);
        }
    }
}
