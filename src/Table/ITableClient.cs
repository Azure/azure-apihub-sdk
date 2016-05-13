using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Sdk.Common;


namespace Microsoft.Azure.ApiHub.Sdk.Table
{
    /// <summary>
    /// Defines the table client.
    /// </summary>
    public interface ITableClient
    {

        /// <summary>
        /// Gets a reference to a data set.
        /// </summary>
        /// <param name="dataSetName">The name of the data set.</param>
        /// <returns>The data set reference.</returns>
        IDataSet GetDataSetReference(string dataSetName = null);

        /// <summary>
        /// Queries the table client for data sets.
        /// </summary>
        /// <param name="query">The query to be executed.</param>
        /// <param name="continuationToken">A continuation token from the server 
        /// when the operation returns a partial result.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The retrieved data sets. In case of partial result the
        /// object returned will have a continuation token.</returns>
        Task<SegmentedResult<IDataSet>> ListDataSetsAsync(
            Query query = null,
            ContinuationToken continuationToken = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
