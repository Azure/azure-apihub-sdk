using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub
{
    /// <summary>
    /// Defines a set of tables.
    /// </summary>
    public interface IDataSet
    {
        /// <summary>
        /// Gets the data set name.
        /// </summary>
        string DataSetName { get; }

        /// <summary>
        /// Gets the data set display name.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets a reference to a table.
        /// </summary>
        /// <typeparam name="TEntity">The type of entities in the table.</typeparam>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The table reference.</returns>
        ITable<TEntity> GetTableReference<TEntity>(string tableName)
            where TEntity : class;

        /// <summary>
        /// Queries the data set for tables.
        /// </summary>
        /// <param name="query">The query to be executed.</param>
        /// <param name="continuationToken">A continuation token from the server 
        /// when the operation returns a partial result.</param>
        /// <param name="cancellationToken">A cancellation token that can be used 
        /// by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The retrieved tables. In case of partial result the
        /// object returned will have a continuation token.</returns>
        Task<SegmentedResult<ITable<JObject>>> ListTablesAsync(
            Query query = null, 
            ContinuationToken continuationToken = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
