using Microsoft.Azure.ApiHub.Management.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CoreUriTemplate = UriTemplate.Core.UriTemplate;

namespace Microsoft.Azure.ApiHub.Management
{
    public partial class ApiHubClient
    {
        public async Task<ApiId[]> GetManagedApis()
        {
            // https://management.azure.com/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/providers/Microsoft.Web/locations/brazilsouth/managedApis?api-version=2015-08-01-preview"

            const string template = "/subscriptions/{subscriptionId}/providers/Microsoft.Web/locations/{location}/managedApis/?api-version={apiVersion}";
            var parameters = new Dictionary<string, string>();
            parameters["subscriptionId"] = this.subscriptionId;
            parameters["location"] = this.location;
            parameters["apiVersion"] = ApiVersion;

            Uri templateUri = new Uri(_managementEndpointUri, template);
            Uri url = new CoreUriTemplate(templateUri.OriginalString).BindByName(parameters);

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
