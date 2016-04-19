using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub.Extensions
{

    /// <summary>
    /// This class contains helper methods for HTTP Client.  
    /// </summary>
    public static class HttpClientExtensions
    {
        #region Synchronous methods
        /// <summary>
        /// Sends a PUT request to the server with the provided JSON body synchronously.
        /// </summary>
        /// <typeparam name="T">The type being sent.</typeparam>
        /// <param name="client">The client to use for the request.</param>
        /// <param name="requestUri">The URI for the request.</param>
        /// <param name="body">The JSON body for the put request.</param>
        public static HttpResponseMessage PutAsJsonSync<T>(this HttpClient client, string requestUri, T body)
        {
            return client.PutAsync(requestUri: requestUri, content: new StringContent(body.ToJson(), Encoding.UTF8, "application/json")).Result;
        }

        /// <summary>
        /// Sends a POST request to the server with the provided JSON body synchronously.
        /// </summary>
        /// <typeparam name="T">The type being sent.</typeparam>
        /// <param name="client">The client to use for the request.</param>
        /// <param name="requestUri">The URI for the request.</param>
        /// <param name="body">The JSON body for the post request.</param>
        public static HttpResponseMessage PostAsJsonSync<T>(this HttpClient client, string requestUri, T body)
        {
            return client.PostAsync(requestUri: requestUri, content: new StringContent(body.ToJson(), Encoding.UTF8, "application/json")).Result;
        }

        /// <summary>
        /// Synchronously reads the JSON content from the http response message.
        /// </summary>
        /// <typeparam name="T">The type of object contained in the JSON.</typeparam>
        /// <param name="message">The response message to be read.</param>
        public static T ReadAsJsonSync<T>(this HttpResponseMessage message)
        {
            return message.Content.ReadAsStringAsync().Result.FromJson<T>(JsonExtensions.MediaTypeFormatterSettings);
        }

        /// <summary>
        /// Synchronously reads the JSON content from the http request message.
        /// </summary>
        /// <typeparam name="T">The type of object contained in the JSON.</typeparam>
        /// <param name="message">The request message to be read.</param>
        public static T ReadAsJsonSync<T>(this HttpRequestMessage message)
        {
            return message.Content.ReadAsStringAsync().Result.FromJson<T>(JsonExtensions.MediaTypeFormatterSettings);
        }

        public static string GetHeader(this HttpResponseMessage response, string name)
        {
            IEnumerable<string> x;
            response.Headers.TryGetValues(name, out x);
            if (x != null && x.Any())
            {
                return x.First();
            }
            return null;
        }
        #endregion Synchronous methods

        #region Asynchronous methods
        /// <summary>
        /// Sends a PUT request to the server with the provided JSON body asynchronously.
        /// </summary>
        /// <typeparam name="T">The type being sent.</typeparam>
        /// <param name="client">The client to use for the request.</param>
        /// <param name="requestUri">The URI for the request.</param>
        /// <param name="body">The JSON body for the put request.</param>
        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, string requestUri, T body)
        {
            return client.PutAsync(requestUri: requestUri, content: new StringContent(body.ToJson(), Encoding.UTF8, "application/json"));
        }

        /// <summary>
        /// Sends a POST request to the server with the provided JSON body asynchronously.
        /// </summary>
        /// <typeparam name="T">The type being sent.</typeparam>
        /// <param name="client">The client to use for the request.</param>
        /// <param name="requestUri">The URI for the request.</param>
        /// <param name="body">The JSON body for the post request.</param>
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string requestUri, T body)
        {
            return client.PostAsync(requestUri: requestUri, content: new StringContent(body.ToJson(), Encoding.UTF8, "application/json"));
        }

        /// <summary>
        /// Asynchronously reads the JSON content from the http response message.
        /// </summary>
        /// <typeparam name="T">The type of object contained in the JSON.</typeparam>
        /// <param name="message">The response message to be read.</param>
        public static async Task<T> ReadAsJsonAsync<T>(this HttpResponseMessage message)
        {
            string content = await message.Content.ReadAsStringAsync();
            return content.FromJson<T>(JsonExtensions.MediaTypeFormatterSettings);
        }

        /// <summary>
        /// Asynchronously reads the JSON content from the http request message.
        /// </summary>
        /// <typeparam name="T">The type of object contained in the JSON.</typeparam>
        /// <param name="message">The request message to be read.</param>
        public static async Task<T> ReadAsJsonAsync<T>(this HttpRequestMessage message)
        {
            string content = await message.Content.ReadAsStringAsync();
            return content.FromJson<T>(JsonExtensions.MediaTypeFormatterSettings);
        }
        #endregion Asynchronous methods
    }
}
