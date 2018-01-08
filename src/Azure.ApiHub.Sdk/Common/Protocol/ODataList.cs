// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;

namespace Microsoft.Azure.ApiHub.Common.Protocol
{
    internal class ODataList<TItem>
    {
        [JsonProperty(ODataConstants.NextLinkProperty)]
        public string NextLink { get; set; }

        [JsonProperty(ODataConstants.ValueProperty)]
        public TItem[] Items { get; set; }
    }
}
