using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Sdk.Common;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub.Sdk.Table.Internal
{
    public class DataSet : IDataSet
    {
        public string DataSetName { get; private set; }

        public string DisplayName { get; internal set; }

        private ITabularConnectorAdapter Adapter { get; set; }

        public DataSet(string dataSetName, ITabularConnectorAdapter adapter)
        {
            if (string.IsNullOrEmpty(dataSetName))
            {
                throw new ArgumentException("The dataset name must not be null or empty.", "dataSetName");
            }

            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }

            DataSetName = dataSetName;
            Adapter = adapter;
        }

        public ITable<TEntity> GetTableReference<TEntity>(string tableName)
            where TEntity : class
        {
            return new Table<TEntity>(DataSetName, tableName, Adapter);
        }

        public async Task<SegmentedResult<ITable<JObject>>> ListTablesAsync(
            Query query,
            ContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            return await Adapter.ListTablesAsync(
                DataSetName, 
                query, 
                continuationToken, 
                cancellationToken);
        }
    }
}
