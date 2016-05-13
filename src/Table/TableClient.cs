using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Sdk.Common;
using Microsoft.Azure.ApiHub.Sdk.Table.Protocol;

namespace Microsoft.Azure.ApiHub.Sdk.Table
{
    internal class TableClient : ITableClient
    {
        private ITabularConnectorAdapter Adapter { get; set; }

        public TableClient(ITabularConnectorAdapter adapter)
        {
            Adapter = adapter;
        }

        public IDataSet GetDataSetReference(string dataSetName)
        {
            return new DataSet(dataSetName ?? Constants.DefaultDataSetName, Adapter);
        }

        public async Task<SegmentedResult<IDataSet>> ListDataSetsAsync(
            Query query,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            return await Adapter.ListDataSetsAsync(
                query, 
                continuationToken, 
                cancellationToken);
        }
    }
}
