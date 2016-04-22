using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub.Sdk.Tabular
{
    public class TableFilterRestrictionsMetadata
    {
        internal TableFilterRestrictionsMetadata()
        {            
        }

        public bool Filterable { get; internal set; }

        public IReadOnlyList<string> NonFilterableProperties { get; internal set; }

        public IReadOnlyList<string> RequiredProperties { get; internal set; }
    }
}
