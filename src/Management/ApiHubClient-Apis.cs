using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Extensions;
using Microsoft.Azure.ApiHub.Management.Entities;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub.Management
{
    public partial class ApiHubClient
    {
        public async Task<ApiId[]> GetManagedApis()
        {
            // https://management.azure.com/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/providers/Microsoft.Web/locations/brazilsouth/managedApis?api-version=2015-08-01-preview"

            UriTemplate apisUrl = new UriTemplate("/subscriptions/{subscriptionId}/providers/Microsoft.Web/locations/{location}/managedApis/?api-version={apiVersion}");
            var parameters = new Dictionary<string, string>();
            parameters["subscriptionId"] = this.subscriptionId;
            parameters["location"] = this.location;
            parameters["apiVersion"] = ApiVersion;
            Uri url = apisUrl.BindByName(_managementEndpointUri, parameters);

            var json = await SendAsync<JObject>(HttpMethod.Get, url);
            var apis = json["value"].ToObject<ArmEnvelope<JToken>[]>();

            return Array.ConvertAll(apis, x => new ApiId
            {
                Id = x.Id,
                Name = x.Name
            });
        }
    }
}
