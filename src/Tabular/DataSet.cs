using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Sdk.Common;
using Microsoft.Azure.ApiHub.Sdk.Extensions;
using Microsoft.Azure.ApiHub.Sdk.Tabular.Internal;

namespace Microsoft.Azure.ApiHub.Sdk.Tabular
{
    public class DataSet : IDataSet
    {
        public string DataSetName { get; private set; }

        public string DisplayName { get; internal set; }

        internal TabularConnectorAdapter Adapter { get; private set; }

        internal DataSet(string dataSetName, TabularConnectorAdapter adapter)
        {
            DataSetName = dataSetName;
            Adapter = adapter;
        }

        public Table GetTableReference(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("The table name must not be null or empty.", "tableName");
            }

            return new Table(DataSetName, tableName, Adapter);
        }

        public async Task<SegmentedResult<ITable>> ListTablesAsync(
            Query query = null,
            ContinuationToken continuationToken = null)
        {
            var requestUri = Adapter.Client.CreateRequestUri(
                template: Protocol.Constants.TablesTemplate,
                parameters: new NameValueCollection
                {
                    {Protocol.Constants.DataSetNameParameter, DataSetName}
                },
                query: query);

            var result = await Adapter.Client.ListAsync<Protocol.Table>(requestUri);

            return new SegmentedResult<ITable>
            {
                Items = result.Items.Coalesce()
                    .Select(item => Adapter.ProtocolToModel.Convert(item, this))
                    .ToList()
            };
        }
    }
}
