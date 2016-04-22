using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Sdk.Common;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub.Sdk.Tabular
{
    public interface ITable
    {
        string TableName { get; }

        string DisplayName { get; }

        Task<TableMetadata> GetMetadataAsync();

        Task<JObject> GetEntityAsync(string entityId);

        Task<SegmentedResult<JObject>> ListEntitiesAsync(
            Query query = null,
            ContinuationToken continuationToken = null);

        Task CreateEntityAsync(JObject entity);

        Task UpdateEntityAsync(string entityId, JObject entity);

        Task DeleteEntityAsync(string entityId);
    }
}
