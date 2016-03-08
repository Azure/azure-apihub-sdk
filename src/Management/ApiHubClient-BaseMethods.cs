using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microfoft.Azure.ApiHub.Sdk.Extensions;
using Newtonsoft.Json.Linq;

namespace Microfoft.Azure.ApiHub.Sdk.Management
{
    public partial class ApiHubClient
    {
        private static HttpClient _client = new HttpClient();

        private static string _managementEndpoint = "https://management.azure.com";
        private Uri _managementEndpointUri = new Uri(_managementEndpoint);

        private string ApiVersion = "2015-08-01-preview"; // add to query string
        private string subscriptionId;
        private string location;
        private string aadToken;

        private void AddAccessToken(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.aadToken);
        }

        protected async Task<TResult> SendAsync<TResult>(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);
            AddAccessToken(request);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Azure ARM REST call failed: " + response.StatusCode);
            }

            var json = await response.ReadAsJsonAsync<JObject>();
            return json.ToObject<TResult>();
        }

        protected async Task<TResult> SendAsync<TResult>(HttpMethod method, Uri url)
        { 
            var request = new HttpRequestMessage(method, url);
            AddAccessToken(request);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Azure ARM REST call failed: " + response.StatusCode);
            }

            var json = await response.ReadAsJsonAsync<JObject>();
            return json.ToObject<TResult>();
        }

        public ApiHubClient(string subscriptionId, string location, string aadToken)
        {
            this.subscriptionId = subscriptionId;
            this.location = location;
            this.aadToken = aadToken;
        }
    }
}
