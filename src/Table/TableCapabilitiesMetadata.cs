using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub.Sdk.Table
{
    public class TableCapabilitiesMetadata
    {
        public TableSortRestrictionsMetadata SortRestrictions { get; set; }

        public TableFilterRestrictionsMetadata FilterRestrictions { get; set; }

        public IReadOnlyList<CapabilityFilterFunction> FilterFunctions { get; set; }
    }
}
