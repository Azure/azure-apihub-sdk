// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub
{
    public class TableSortRestrictionsMetadata
    {
        public bool Sortable { get; set; }

        public IReadOnlyList<string> UnsortableProperties { get; set; }

        public IReadOnlyList<string> AscendingOnlyProperties { get; set; }
    }
}
