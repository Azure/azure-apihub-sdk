using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub.Sdk.Table
{
    public class TableSortRestrictionsMetadata
    {
        internal TableSortRestrictionsMetadata()
        {            
        }

        public bool Sortable { get; internal set; }

        public IReadOnlyList<string> UnsortableProperties { get; internal set; }

        public IReadOnlyList<string> AscendingOnlyProperties { get; internal set; }
    }
}
