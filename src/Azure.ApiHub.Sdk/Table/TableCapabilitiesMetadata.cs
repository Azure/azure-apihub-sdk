// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub
{
    public class TableCapabilitiesMetadata
    {
        public TableSortRestrictionsMetadata SortRestrictions { get; set; }

        public TableFilterRestrictionsMetadata FilterRestrictions { get; set; }

        public IReadOnlyList<CapabilityFilterFunction> FilterFunctions { get; set; }
    }
}
