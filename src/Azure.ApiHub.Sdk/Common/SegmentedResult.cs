// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Azure.ApiHub
{
    public class SegmentedResult<TItem>
    {
        public IReadOnlyList<TItem> Items { get; set; }

        public ContinuationToken ContinuationToken { get; set; }
    }
}
