using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Sdk.Common;
using Microsoft.Azure.ApiHub.Sdk.Extensions;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Microsoft.Azure.ApiHub.Sdk.Table.Internal
{
    internal class TabularConnectorAdapter : ITabularConnectorAdapter
    {
        private ConnectorHttpClient HttpClient { get; set; }

        private ProtocolToModelConverter ProtocolToModel { get; set; }

        public TabularConnectorAdapter(
            ConnectorHttpClient httpClient, 
            ProtocolToModelConverter protocolToModel)
        {
            HttpClient = httpClient;
            ProtocolToModel = protocolToModel;
        }

        public async Task<SegmentedResult<IDataSet>> ListDataSetsAsync(
            Query query,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            var requestUri = HttpClient.CreateRequestUri(
                template: Protocol.Constants.DataSetsTemplate);

            var result = await HttpClient.ListAsync<Protocol.DataSet>(requestUri, cancellationToken);

            return new SegmentedResult<IDataSet>
            {
                Items = result.Items.Coalesce()
                    .Select(dataSet => ProtocolToModel.Convert(dataSet, this))
                    .ToList()
            };
        }

        public async Task<SegmentedResult<ITable<JObject>>> ListTablesAsync(
            string dataSetName,
            Query query,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            var requestUri = HttpClient.CreateRequestUri(
                template: Protocol.Constants.TablesTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, dataSetName}
                },
                query: query);

            var result = await HttpClient.ListAsync<Protocol.Table>(requestUri, cancellationToken);

            return new SegmentedResult<ITable<JObject>>
            {
                Items = result.Items.Coalesce()
                    .Select(table => ProtocolToModel.Convert<JObject>(table, dataSetName, this))
                    .ToList()
            };
        }

        public async Task<TableMetadata> GetTableMetadataAsync(
            string dataSetName,
            string tableName,
            CancellationToken cancellationToken)
        {
            var requestUri = HttpClient.CreateRequestUri(
                template: Protocol.Constants.TableMetadataTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, dataSetName},
                    {Protocol.Constants.TableNameParameter, tableName}
                });

            var metadata = await HttpClient.GetAsync<Protocol.TableMetadata>(requestUri, cancellationToken);

            return metadata != null ? ProtocolToModel.Convert(metadata) : null;
        }

        public async Task<TEntity> GetEntityAsync<TEntity>(
            string dataSetName, 
            string tableName, 
            string entityId,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            var requestUri = HttpClient.CreateRequestUri(
                template: Protocol.Constants.TableItemTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, dataSetName},
                    {Protocol.Constants.TableNameParameter, tableName},
                    {Protocol.Constants.ItemIdParameter, entityId}
                });

            var entity = await HttpClient.GetAsync<TEntity>(requestUri, cancellationToken);

            return entity;
        }

        public async Task<SegmentedResult<TEntity>> ListEntitiesAsync<TEntity>(
            string dataSetName,
            string tableName,
            Query query,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            var requestUri = HttpClient.CreateRequestUri(
                template: Protocol.Constants.TableItemsTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, dataSetName},
                    {Protocol.Constants.TableNameParameter, tableName}
                },
                query: query);

            var result = await HttpClient.ListAsync<TEntity>(requestUri, cancellationToken);

            return new SegmentedResult<TEntity>
            {
                Items = result.Items.Coalesce()
            };
        }

        public async Task CreateEntityAsync<TEntity>(
            string dataSetName,
            string tableName,
            TEntity entity,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            var requestUri = HttpClient.CreateRequestUri(
                template: Protocol.Constants.TableItemsTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, dataSetName},
                    {Protocol.Constants.TableNameParameter, tableName}
                });

            await HttpClient.PostAsync(requestUri, entity, cancellationToken);
        }

        public async Task UpdateEntityAsync<TEntity>(
            string dataSetName,
            string tableName,
            string entityId, 
            TEntity entity,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            var requestUri = HttpClient.CreateRequestUri(
                template: Protocol.Constants.TableItemTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, dataSetName},
                    {Protocol.Constants.TableNameParameter, tableName},
                    {Protocol.Constants.ItemIdParameter, entityId}
                });

            await HttpClient.PatchAsync(requestUri, entity, cancellationToken);
        }

        public async Task DeleteEntityAsync(
            string dataSetName,
            string tableName,
            string entityId,
            CancellationToken cancellationToken)
        {
            var requestUri = HttpClient.CreateRequestUri(
                template: Protocol.Constants.TableItemTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, dataSetName},
                    {Protocol.Constants.TableNameParameter, tableName},
                    {Protocol.Constants.ItemIdParameter, entityId}
                });

            await HttpClient.DeleteAsync(requestUri, cancellationToken);
        }
    }
}
