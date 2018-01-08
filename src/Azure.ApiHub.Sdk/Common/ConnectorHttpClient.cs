// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Extensions;
using Newtonsoft.Json;
using CoreUriTemplate = UriTemplate.Core.UriTemplate;

namespace Microsoft.Azure.ApiHub.Common
{
    // TODO: Add retry functionality.
    internal class ConnectorHttpClient
    {
        private const string MediaTypeApplicationJson = "application/json";

        public ConnectorHttpClient(Uri runtimeEndpoint, string accessTokenScheme, string accessToken)
        {
            HttpClient = new HttpClient();
            RuntimeEndpoint = runtimeEndpoint;
            AccessTokenScheme = accessTokenScheme;
            AccessToken = accessToken;
        }

        private HttpClient HttpClient { get; set; }

        private Uri RuntimeEndpoint { get; set; }

        private string AccessTokenScheme { get; set; }

        private string AccessToken { get; set; }

        // TODO: Use continuation token.
        public virtual Uri CreateRequestUri(string template, NameValueCollection parameters = null, Query query = null, ContinuationToken continuationToken = null)
        {
            var paramatersContainer = new Dictionary<string, string>();
            if (parameters != null)
            {
                foreach (var key in parameters.AllKeys)
                {
                    paramatersContainer.Add(key, parameters[key]);
                }
            }
            
            var uriTemplate = new CoreUriTemplate(template)
                .BindByName(RuntimeEndpoint, paramatersContainer);
            
            var uriBuilder = new UriBuilder(uriTemplate.AbsoluteUri)
            {
                Query = query.Coalesce().QueryString
            };

            return uriBuilder.Uri;
        }

        public virtual async Task<TItem> GetAsync<TItem>(Uri requestUri, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            AddRequestHeaders(request);

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

        public virtual async Task<Protocol.ODataList<TItem>> ListAsync<TItem>(Uri requestUri, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            AddRequestHeaders(request);

            var response = await HttpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.ReadAsJsonAsync<Protocol.ODataList<TItem>>(
                JsonExtensions.ObjectSerializationSettings);

            return result;
        }

        public virtual async Task PostAsync<TItem>(Uri requestUri, TItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            AddRequestHeaders(request);

            request.Content = new StringContent(
                JsonConvert.SerializeObject(item),
                Encoding.UTF8,
                MediaTypeApplicationJson);

            var response = await HttpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public virtual async Task PatchAsync<TItem>(Uri requestUri, TItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri);

            AddRequestHeaders(request);

            request.Content = new StringContent(
                JsonConvert.SerializeObject(item),
                Encoding.UTF8,
                MediaTypeApplicationJson);

            var response = await HttpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public virtual async Task DeleteAsync(Uri requestUri, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);

            AddRequestHeaders(request);

            var response = await HttpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            response.EnsureSuccessStatusCode();
        }

        protected virtual void AddRequestHeaders(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(AccessTokenScheme, AccessToken);
            request.Headers.UserAgent.TryParseAdd(Extensions.HttpClientExtensions.GetUserAgent());
        }
    }
}
