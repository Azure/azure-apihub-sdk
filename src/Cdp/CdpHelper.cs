using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microfoft.WindowsAzure.ApiHub
{
    internal class CdpHelper
    {
        private static HttpClient _httpClient = new HttpClient();

        private Uri _runtimeEndpoint;
        private string _accessTokenScheme;
        private string _accessToken;

        public Uri RuntimeEndpoint
        {
            get
            {
                return _runtimeEndpoint;
            }
        }

        public string AccessTokenScheme
        {
            get
            {
                return _accessTokenScheme;
            }
        }

        public string AccessToken
        {
            get
            {
                return _accessToken;
            }
        }

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
                throw new HttpRequestException("Request failed for: " + url.AbsolutePath + " message: " + ex.Message);
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
                if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

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
            if(response.Content == null)
            {
                return default(TResult);
            }

            string json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
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

        private void AddAccessToken(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(_accessTokenScheme, _accessToken);
        }
    }
}
