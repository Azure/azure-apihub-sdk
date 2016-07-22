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
