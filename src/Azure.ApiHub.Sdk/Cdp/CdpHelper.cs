﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Azure.ApiHub
{
    internal class CdpHelper
    {
        private static HttpClient _httpClient = new HttpClient();

        private Uri _runtimeEndpoint;
        private string _accessTokenScheme;
        private string _accessToken;
        private ILogger _logger;
        private string _additionalContext;

        static CdpHelper()
        {
            // Setting the timeout to a very high value to make sure the timeouts of the underlying APIM and connectors are honored.
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
        }

        public CdpHelper(Uri runtimeEndpoint, string scheme, string accessToken, ILogger logger, string additionalContext = null)
        {
            _runtimeEndpoint = runtimeEndpoint;
            _accessTokenScheme = scheme;
            _accessToken = accessToken;
            _logger = logger;
            _additionalContext = additionalContext;
        }

        public Uri RuntimeEndpoint => _runtimeEndpoint;
        
        public string AccessTokenScheme => _accessTokenScheme;

        public string AccessToken => _accessToken;

        public ILogger Logger => _logger;
        
        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, Uri url, HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseContentRead, byte[] content = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            AddRequestHeaders(request);

            HttpResponseMessage response = null;
            try
            {
                if (content != null)
                {
                    request.Content = new ByteArrayContent(content);
                }

                response = await _httpClient.SendAsync(request, httpCompletionOption);

                if (!response.IsSuccessStatusCode)
                {
                    await LogNonSuccessAsync(response);
                }
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Request failed for: " + url.AbsolutePath, ex);
            }
            return response;
        }

        public async Task<Tuple<TResult, HttpStatusCode>> SendResultAsync<TResult>(HttpMethod method, Uri url, byte[] content = null)
        {
            HttpResponseMessage response = await SendAsync(method, url, content: content);

            TResult result = default(TResult);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = await DecodeAsync<TResult>(response);
            }
            else
            {
                await LogNonSuccessAsync(response);
            }

            return new Tuple<TResult, HttpStatusCode>(result, response.StatusCode);
        }

        public async Task<byte[]> SendRawAsync(HttpMethod method, Uri url, byte[] content = null)
        {
            HttpResponseMessage response = await SendAsync(method, url, content: content);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                await LogNonSuccessAsync(response);
                throw new HttpRequestException("Request failed for: " + url.AbsolutePath + "response code: " + response.StatusCode);
            }

            if (response.Content != null)
            {
                var result = await response.Content.ReadAsByteArrayAsync();
                return result;
            }
            else
            {
                // empty content
                return new byte[0];
            }
        }

        public async Task<TResult> DecodeAsync<TResult>(HttpResponseMessage response)
        {
            if (response.Content == null)
            {
                return default(TResult);
            }

            string json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                await LogNonSuccessAsync(response);
                throw new HttpRequestException("Request failed response code: " + response.StatusCode);
            }
            var result = JsonConvert.DeserializeObject<TResult>(json);
            return result;
        }

        public Uri MakeUri(string url, params object[] args)
        {
            string x = string.Format(url, args);
            return new Uri(this.RuntimeEndpoint.ToString() + x);
        }

        private void AddRequestHeaders(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(_accessTokenScheme, _accessToken);

            var userAgent = Extensions.HttpClientExtensions.GetUserAgent() + (!string.IsNullOrEmpty(_additionalContext) ? " - " + _additionalContext : string.Empty);
            request.Headers.UserAgent.TryParseAdd(userAgent);
        }

        private async Task LogNonSuccessAsync(HttpResponseMessage response)
        {
            string content = string.Empty;
            if (response.Content != null)
            {
                content = await response.Content.ReadAsStringAsync();
            }

            _logger.Error(string.Format("Request returned status: {0}, verb: {1}, message: {2}, uri: {3}", response.StatusCode, response.RequestMessage.Method, content, response.RequestMessage.RequestUri.AbsoluteUri));
        }
    }
}
