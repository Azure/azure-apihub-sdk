using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub.Sdk.Table
{
    public class TableFilterRestrictionsMetadata
    {
        public bool Filterable { get; set; }

        public IReadOnlyList<string> NonFilterableProperties { get; set; }

        public IReadOnlyList<string> RequiredProperties { get; set; }
    }
}
