// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Management.Entities;
using Newtonsoft.Json.Linq;
using Tavis.UriTemplates;

namespace Microsoft.Azure.ApiHub.Management
{
    public partial class ApiHubClient
    {
        public async Task<ApiId[]> GetManagedApis()
        {
            // https://management.azure.com/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/providers/Microsoft.Web/locations/brazilsouth/managedApis?api-version=2015-08-01-preview"

            const string template = "/subscriptions/{subscriptionId}/providers/Microsoft.Web/locations/{location}/managedApis/?api-version={apiVersion}";
            var parameters = new Dictionary<string, string>();
            parameters["subscriptionId"] = subscriptionId;
            parameters["location"] = location;
            parameters["apiVersion"] = ApiVersion;


            var uriTemplate = new UriTemplate(template, true);

            // Add parameters one by one to avoid mismatch parameters count errors.
            foreach (var key in parameters)
            {
                uriTemplate = uriTemplate.AddParameter(key.Key, key.Value);
            }

            var resolvedUri = uriTemplate.Resolve();

            // Build complete URI.
            Uri url = new Uri(_managementEndpointUri, resolvedUri);
                        
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
