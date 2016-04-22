using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Sdk.Common;
using Microsoft.Azure.ApiHub.Sdk.Extensions;
using Microsoft.Azure.ApiHub.Sdk.Tabular.Internal;

namespace Microsoft.Azure.ApiHub.Sdk.Tabular
{
    public class TableClient : ITableClient
    {
        internal TabularConnectorAdapter Adapter { get; private set; }

        internal TableClient(TabularConnectorAdapter adapter)
        {
            Adapter = adapter;
        }

        public DataSet GetDataSetReference(string dataSetName)
        {
            if (string.IsNullOrEmpty(dataSetName))
            {
                throw new ArgumentException("The dataset name must not be null or empty.", "dataSetName");
            }

            return new DataSet(dataSetName, Adapter);
        }

        public async Task<SegmentedResult<DataSet>> ListDataSetsAsync(
            Query query = null,
            ContinuationToken continuationToken = null)
        {
            var requestUri = Adapter.Client.CreateRequestUri(
                template: Protocol.Constants.DataSetsTemplate);

            var result = await Adapter.Client.ListAsync<Protocol.DataSet>(requestUri);

            return new SegmentedResult<DataSet>
            {
                Items = result.Items.Coalesce()
                    .Select(item => Adapter.ProtocolToModel.Convert(item, this))
                    .ToList()
            };
        }
    }
}
