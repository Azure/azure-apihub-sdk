using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Sdk.Common;

namespace Microsoft.Azure.ApiHub.Sdk.Tabular
{
    public interface IDataSet
    {
        string DataSetName { get; }

        Table GetTableReference(string tableName);

        Task<SegmentedResult<ITable>> ListTablesAsync(
            Query query = null, 
            ContinuationToken continuationToken = null);
    }
}
