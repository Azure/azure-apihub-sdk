// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub
{
    public class TableFilterRestrictionsMetadata
    {
        public bool Filterable { get; set; }

        public IReadOnlyList<string> NonFilterableProperties { get; set; }

        public IReadOnlyList<string> RequiredProperties { get; set; }
    }
}
