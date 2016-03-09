using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microfoft.Azure.ApiHub.Sdk.Cdp
{
    internal class CdpHelper
    {
        private static HttpClient _httpClient = new HttpClient();

        private Uri _runtimeEndpoint;
        private string _accessTokenScheme;
        private string _accessToken;

        public CdpHelper(Uri runtimeEndpoint, string scheme, string accessToken)
        {
            _runtimeEndpoint = runtimeEndpoint;
            _accessTokenScheme = scheme;
            _accessToken = accessToken;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, Uri url, HttpContent content = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            AddAccessToken(request);

            HttpResponseMessage response = null;
            try
            {
                if (content != null)
                {
                    request.Content = content;
                }

                response = await _httpClient.SendAsync(request);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return response;
        }

        public async Task<TREsult> SendResultAsync<TREsult>(HttpMethod method, Uri url, HttpContent content = null)
        {
            HttpResponseMessage response = await SendAsync(method, url, content);
            var result = await DecodeAsync<TREsult>(response);
            return result;
        }

        public async Task<byte[]> SendRawAsync(HttpMethod method, Uri url, HttpContent content = null)
        {
            HttpResponseMessage response = await SendAsync(method, url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("REST call failed");
            }
            var result = await response.Content.ReadAsByteArrayAsync();
            return result;
        }

        public async Task<TResult> DecodeAsync<TResult>(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("REST call failed");
            }
            var result = JsonConvert.DeserializeObject<TResult>(json);
            return result;
        }

        public string ToConnectionString()
        {
            return string.Format("{0};{1};{2}", _runtimeEndpoint, _accessTokenScheme, _accessToken);
        }

        public Uri MakeUri(string url, params object[] args)
        {
            string x = string.Format(url, args);
            return new Uri(this._runtimeEndpoint.ToString() + x);
        }

        private void AddAccessToken(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(_accessTokenScheme, _accessToken);
        }
    }
}
