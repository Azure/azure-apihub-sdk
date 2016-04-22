using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Sdk.Common;
using Microsoft.Azure.ApiHub.Sdk.Extensions;
using Microsoft.Azure.ApiHub.Sdk.Tabular.Internal;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub.Sdk.Tabular
{
    public class Table : ITable
    {
        public string DataSetName { get; internal set; }

        public string TableName { get; internal set; }

        public string DisplayName { get; internal set; }

        internal TabularConnectorAdapter Adapter { get; private set; }

        internal Table(string dataSetName, string tableName, TabularConnectorAdapter adapter)
        {
            DataSetName = dataSetName;
            TableName = tableName;
            Adapter = adapter;
        }

        public async Task<TableMetadata> GetMetadataAsync()
        {
            var requestUri = Adapter.Client.CreateRequestUri(
                template: Protocol.Constants.TableMetadataTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, DataSetName},
                    {Protocol.Constants.TableNameParameter, TableName}
                });

            var metadata = await Adapter.Client.GetAsync<Protocol.TableMetadata>(requestUri);

            return Adapter.ProtocolToModel.Convert(metadata);
        }

        public async Task<JObject> GetEntityAsync(string entityId)
        {
            var requestUri = Adapter.Client.CreateRequestUri(
                template: Protocol.Constants.TableItemTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, DataSetName},
                    {Protocol.Constants.TableNameParameter, TableName},
                    {Protocol.Constants.ItemIdParameter, entityId}
                });

            var entity = await Adapter.Client.GetAsync<JObject>(requestUri);

            return entity;
        }

        public async Task<SegmentedResult<JObject>> ListEntitiesAsync(
            Query query = null,
            ContinuationToken continuationToken = null)
        {
            var requestUri = Adapter.Client.CreateRequestUri(
                template: Protocol.Constants.TableItemsTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, DataSetName},
                    {Protocol.Constants.TableNameParameter, TableName}
                },
                query: query);

            var result = await Adapter.Client.ListAsync<JObject>(requestUri);

            return new SegmentedResult<JObject>
            {
                Items = result.Items.Coalesce()
            };
        }

        public async Task CreateEntityAsync(JObject entity)
        {
            var requestUri = Adapter.Client.CreateRequestUri(
                template: Protocol.Constants.TableItemsTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, DataSetName},
                    {Protocol.Constants.TableNameParameter, TableName}
                });

            await Adapter.Client.PostAsync(requestUri, entity);
        }

        public async Task UpdateEntityAsync(string entityId, JObject entity)
        {
            var requestUri = Adapter.Client.CreateRequestUri(
                template: Protocol.Constants.TableItemTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, DataSetName},
                    {Protocol.Constants.TableNameParameter, TableName},
                    {Protocol.Constants.ItemIdParameter, entityId}
                });

            await Adapter.Client.PatchAsync(requestUri, entity);
        }

        public async Task DeleteEntityAsync(string entityId)
        {
            var requestUri = Adapter.Client.CreateRequestUri(
                template: Protocol.Constants.TableItemTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, DataSetName},
                    {Protocol.Constants.TableNameParameter, TableName},
                    {Protocol.Constants.ItemIdParameter, entityId}
                });

            await Adapter.Client.DeleteAsync(requestUri);
        }
    }
}
