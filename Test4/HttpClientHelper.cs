using System;
using System.Collections.Generic;

using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test4
{
    /// <summary>
    /// Helper class of http client (support http post and get)
    /// <br/>
    /// <br>Example: Use bing search, request params [q=123]</br>
    /// <br></br>
    /// var str = HttpClientHelper.GetString(&quot;https://cn.bing.com/search&quot;,
    ///   new List&lt;KeyValuePair&lt;string, string&gt;&gt;()
    ///   {new KeyValuePair&lt;string, string&gt;(&quot;q&quot;, &quot;124&quot;)})
    /// </summary>
    public static class HttpClientHelper
    {
        #region Singleton of HttpClient

        private static readonly object Locker = new object();
        private static HttpClient _httpClient;

        /// <summary>
        /// Progress handler for get/post
        /// </summary>
        public static readonly ProgressMessageHandler Handler
            = new ProgressMessageHandler(new HttpClientHandler());

        /// <summary>
        /// singleton of http client
        /// </summary>
        /// <remarks>Base url has already setting</remarks>
        /// <returns></returns>
        private static HttpClient GetClient()
        {
            if (_httpClient == null)
            {
                lock (Locker)
                {
                    if (_httpClient == null)
                    {
                        _httpClient = new HttpClient(Handler, true);
                    }
                }
            }

            return _httpClient;
        }

        #endregion

        #region Http Post

        /// <summary>
        /// Get http request result as post method
        /// </summary>
        /// <param name="url">Partial url or full url</param>
        /// <param name="parameters">Request params</param>
        /// <param name="headers">Request header</param>
        /// <returns>IO stream</returns>
        public static Stream Post(
            string url,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            return PostAsync(url, parameters, headers,
                CancellationToken.None).Result;
        }

        /// <summary>
        /// Get http request result as post method
        /// </summary>
        /// <param name="url">Partial url or full url</param>
        /// <param name="parameters">Request params</param>
        /// <param name="headers">Request header</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>IO stream</returns>
        public static async Task<Stream> PostAsync(
            string url,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null,
            CancellationToken cancellationToken = default)
        {
            var responseMsg = await SendAsync(url, HttpMethod.Post, parameters,
                headers, cancellationToken);
            return await responseMsg.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Get http request result as post method
        /// </summary>
        /// <param name="url">Partial url or full url</param>
        /// <param name="parameters">Request params</param>
        /// <param name="headers">Request header</param>
        /// <returns>String</returns>
        public static string PostString(
            string url,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            using (var stream = Post(url, parameters, headers))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Get http request result as post method
        /// </summary>
        /// <param name="url">Partial url or full url</param>
        /// <param name="parameters">Request params</param>
        /// <param name="headers">Request header</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>String</returns>
        public static async Task<string> PostStringAsync(
            string url,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null,
            CancellationToken cancellationToken = default)
        {
            using (var stream = await PostAsync(url, parameters,
                headers, cancellationToken))
            {
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        #endregion

        #region Http Get

        /// <summary>
        /// Get http request result as get method
        /// </summary>
        /// <param name="url">Partial url or full url</param>
        /// <param name="parameters">Request params</param>
        /// <param name="headers">Request header</param>
        /// <returns>String</returns>
        public static string GetString(
            string url,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            using (var stream = Get(url, parameters, headers))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Get http request result as get method
        /// </summary>
        /// <param name="url">Partial url or full url</param>
        /// <param name="parameters">Request params</param>
        /// <param name="headers">Request header</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>String</returns>
        public static async Task<string> GetStringAsync(
            string url,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null,
            CancellationToken cancellationToken = default)
        {
            using (var stream = await GetAsync(url, parameters,
                headers, cancellationToken))
            {
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        /// <summary>
        /// Get http request result as get method
        /// </summary>
        /// <param name="url">Partial url or full url</param>
        /// <param name="parameters">Request params</param>
        /// <param name="headers">Request header</param>
        /// <returns>IO stream</returns>
        public static Stream Get(
            string url,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            return GetAsync(url, parameters, headers).Result;
        }

        /// <summary>
        /// Get http request result as get method
        /// </summary>
        /// <param name="url">Partial url or full url</param>
        /// <param name="parameters">Request params</param>
        /// <param name="headers">Request header</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>IO stream</returns>
        public static async Task<Stream> GetAsync(
            string url,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null,
            CancellationToken cancellationToken = default)
        {
            var responseMsg = await SendAsync(url, HttpMethod.Get, parameters,
                headers, cancellationToken);
            return await responseMsg.Content.ReadAsStreamAsync();
        }

        #endregion

        #region Helper Method

        /// <summary>
        /// Send a http request with async pattern
        /// </summary>
        /// <remarks>This method with no block</remarks>
        /// <param name="url">Partial url or full url</param>
        /// <param name="method">Http request method get, post, ...</param>
        /// <param name="parameters">Request params</param>
        /// <param name="headers">Request header</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Http response message</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        private static async Task<HttpResponseMessage> SendAsync(
            string url,
            HttpMethod method,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null,
            CancellationToken cancellationToken = default)
        {
            var requestParam = new StringBuilder();
            for (var i = 0; i < parameters?.Count; i++)
            {
                var param = parameters[i].Key + "=" + parameters[i].Value;
                requestParam.Append(param);
                if (i != parameters.Count - 1)
                    requestParam.Append("&");
            }

            var request = new HttpRequestMessage(
                method, (url.Length > 0 && requestParam.Length > 0)
                    ? string.Join("?", url, requestParam.ToString())
                    : url);

            for (var i = 0; i < headers?.Count; i++)
                request.Headers.Add(headers[i].Key, headers[i].Value);

            return (await GetClient().SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                .EnsureSuccessStatusCode();
        }

        #endregion
    }
}