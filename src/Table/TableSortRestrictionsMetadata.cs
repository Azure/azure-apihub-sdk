using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub.Sdk.Table
{
    public class TableSortRestrictionsMetadata
    {
        public bool Sortable { get; set; }

        public IReadOnlyList<string> UnsortableProperties { get; set; }

        public IReadOnlyList<string> AscendingOnlyProperties { get; set; }
    }
}
