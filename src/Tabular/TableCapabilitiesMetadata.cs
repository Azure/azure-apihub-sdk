using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub.Sdk.Tabular
{
    public class TableCapabilitiesMetadata
    {
        internal TableCapabilitiesMetadata()
        {
        }

        public TableSortRestrictionsMetadata SortRestrictions { get; internal set; }

        public TableFilterRestrictionsMetadata FilterRestrictions { get; internal set; }

        public IReadOnlyList<CapabilityFilterFunction> FilterFunctions { get; internal set; }
    }
}
