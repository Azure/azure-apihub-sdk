using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Extensions;
using Microsoft.Azure.ApiHub.Sdk.Extensions;
using Microsoft.Azure.ApiHub.Sdk.Table;
using Newtonsoft.Json;

namespace Microsoft.Azure.ApiHub.Sdk.Common
{
    // TODO: Add retry functionality.
    internal class ConnectorHttpClient
    {
        private const string MediaTypeApplicationJson = "application/json";

        private HttpClient HttpClient { get; set; }

        private Uri RuntimeEndpoint { get; set; }

        private string AccessTokenScheme { get; set; }

        private string AccessToken { get; set; }
        
        public ConnectorHttpClient(Uri runtimeEndpoint, string accessTokenScheme, string accessToken)
        {
            HttpClient = new HttpClient();
            RuntimeEndpoint = runtimeEndpoint;
            AccessTokenScheme = accessTokenScheme;
            AccessToken = accessToken;            
        }

        // TODO: Use continuation token.
        public virtual Uri CreateRequestUri(
            string template, 
            NameValueCollection parameters = null,
            Query query = null,
            ContinuationToken continuationToken = null)
        {
            var uriTemplate = new UriTemplate(template)
                .BindByName(RuntimeEndpoint, parameters.Coalesce());

            var uriBuilder = new UriBuilder(uriTemplate.AbsoluteUri)
            {
                Query = query.Coalesce().QueryString
            };

            return uriBuilder.Uri;
        }

        public virtual async Task<TItem> GetAsync<TItem>(
            Uri requestUri, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            ApplyCredentials(request);

            var response = await HttpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return default(TItem);
            }

            response.EnsureSuccessStatusCode();

            var result = await response.ReadAsJsonAsync<TItem>(
                JsonExtensions.ObjectSerializationSettings);

            return result;
        }

        public virtual async Task<Protocol.ODataList<TItem>> ListAsync<TItem>(
            Uri requestUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            ApplyCredentials(request);

            var response = await HttpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.ReadAsJsonAsync<Protocol.ODataList<TItem>>(
                JsonExtensions.ObjectSerializationSettings);

            return result;
        }

        public virtual async Task PostAsync<TItem>(
            Uri requestUri, 
            TItem item,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            ApplyCredentials(request);

            request.Content = new StringContent(
                JsonConvert.SerializeObject(item), 
                Encoding.UTF8,
                MediaTypeApplicationJson);

            var response = await HttpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public virtual async Task PatchAsync<TItem>(
            Uri requestUri, 
            TItem item,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri);

            ApplyCredentials(request);

            request.Content = new StringContent(
                JsonConvert.SerializeObject(item),
                Encoding.UTF8,
                MediaTypeApplicationJson);

            var response = await HttpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public virtual async Task DeleteAsync(
            Uri requestUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);

            ApplyCredentials(request);

            var response = await HttpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            response.EnsureSuccessStatusCode();
        }

        protected virtual void ApplyCredentials(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(AccessTokenScheme, AccessToken);
        }
    }
}
