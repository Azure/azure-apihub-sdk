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
        public async Task<ConnectionId[]> GetConnectionsAsync(string api)
        {
            // https://management.azure.com/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/providers/Microsoft.Web/locations/brazilsouth/managedApis/dropbox/connections?api-version=2015-08-01-preview"

            const string template = "/subscriptions/{subscriptionId}/providers/Microsoft.Web/locations/{location}/managedApis/{apiName}/connections?api-version={apiVersion}";
            var parameters = new Dictionary<string, string>();
            parameters["apiName"] = api;
            parameters["subscriptionId"] = subscriptionId;
            parameters["location"] = location;
            parameters["apiVersion"] = ApiVersion;
            
            Uri url = BuildUri(_managementEndpointUri, template, parameters);
            
            var json = await SendAsync<JObject>(HttpMethod.Get, url);
            var connections = json["value"].ToObject<ArmEnvelope<JToken>[]>();

            return Array.ConvertAll(connections, x => new ConnectionId
            {
                Id = x.Id,
                ApiName = api,
                Name = x.Name
            });
        }


        private Uri BuildUri(Uri baseUri, string template, IDictionary<string, string> parameters)
        {
            Uri templateUri = new Uri(baseUri, template);
            return new CoreUriTemplate(templateUri.OriginalString).BindByName(parameters);
        }

        public async Task<ConnectionKey> GetConnectionKeyAsync(ConnectionId connection)
        {
            // POST https://management.azure.com/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/resourceGroups/AzureFunctions/providers/Microsoft.Web/connections/1aab7e02-73cd-408c-ba1a-cd273ae48105/listConnectionKeys?api-version=2015-08-01-preview"

            const string template = "{armId}/listConnectionKeys?api-version={apiVersion}";
            var parameters = new Dictionary<string, string>();
            parameters["armId"] = connection.Id;
            parameters["apiVersion"] = ApiVersion;
            Uri url = BuildUri(_managementEndpointUri, template, parameters);

            var result = await SendAsync<JObject>(HttpMethod.Post, url);

            string connectonKey = result["connectionKey"].ToString();
            string runtimeUrl = result["runtimeUrls"].ToObject<string[]>()[0];
            return new ConnectionKey
            {
                Key = connectonKey,
                RuntimeUri = new Uri(runtimeUrl)
            };
        }

        public string GetConnectionString(Uri runtimeEndpoint, string scheme, string accessToken)
        {
            return string.Format(CdpConstants.CdpConnectionStringTemplate, runtimeEndpoint, scheme, accessToken);
        }
    }
}
