using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Sdk.Common;

namespace Microsoft.Azure.ApiHub.Sdk.Tabular
{
    public interface ITableClient
    {
        DataSet GetDataSetReference(string dataSetName);

        Task<SegmentedResult<DataSet>> ListDataSetsAsync(
            Query query = null,
            ContinuationToken continuationToken = null);
    }
}
