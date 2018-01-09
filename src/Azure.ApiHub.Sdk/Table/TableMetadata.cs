// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub
{
    public class TableMetadata
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string Permission { get; set; }

        public TableCapabilitiesMetadata Capabilities { get; set; }

        public JObject Schema { get; set; }
    }
}
