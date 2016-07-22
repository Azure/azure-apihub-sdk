using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub
{
    public class SegmentedResult<TItem>
    {
        public IReadOnlyList<TItem> Items { get; set; }

        public ContinuationToken ContinuationToken { get; set; }
    }
}
