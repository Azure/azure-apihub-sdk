using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub.Sdk.Common
{
    public class SegmentedResult<TItem>
    {
        public IReadOnlyList<TItem> Items { get; set; }

        public string ContinuationToken { get; set; }
    }
}
