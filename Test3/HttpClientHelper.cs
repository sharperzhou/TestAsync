using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IMechProjectManager_HW.Utils
{
    public class HttpClientHelper
    {
        #region Singleton of HttpClient

        private static readonly object Locker = new object();
        private static HttpClient _httpClient = null;

        public static HttpClient GetClient()
        {
            if (_httpClient == null)
            {
                lock (Locker)
                {
                    if (_httpClient == null)
                        _httpClient = new HttpClient();
                }
            }

            return _httpClient;
        }

        #endregion

        #region Http Post

        public static Stream Post(
            string baseUrl,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            return PostAsync(baseUrl, parameters, headers).Result;
        }

        public static async Task<Stream> PostAsync(
            string baseUrl,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            var responseMsg = await PostResponseMsgAsync(baseUrl, parameters, headers);
            return await responseMsg.Content.ReadAsStreamAsync();
        }

        public static string PostString(
            string baseUrl,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            using (var stream = Post(baseUrl, parameters, headers))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static Task<HttpResponseMessage> PostResponseMsgAsync(
            string baseUrl,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            return GetClient().PostAsync(baseUrl,
                MakeUrlEncodedContent(parameters, headers));
        }

        #endregion

        #region Http Get
        public static string GetString(
            string baseUrl,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            using (var stream = Get(baseUrl, parameters, headers))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public static Stream Get(
            string baseUrl,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            return GetAsync(baseUrl, parameters, headers).Result;
        }
        public static async Task<Stream> GetAsync(
            string baseUrl,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            var responseMsg = await GetResponseMsgAsync(baseUrl, parameters, headers);
            return await responseMsg.Content.ReadAsStreamAsync();
        }

        private static Task<HttpResponseMessage> GetResponseMsgAsync(
            string baseUrl,
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            var requestUrl = new StringBuilder();
            for (var i = 0; i < parameters.Count; i++)
            {
                var param = parameters[i].Key + "=" + parameters[i].Value;
                requestUrl.Append(param);
                if (i != parameters.Count - 1)
                    requestUrl.Append("&");
            }

            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Get, baseUrl + "?" + requestUrl.ToString());

            if (headers != null && headers.Count > 0)
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

            return GetClient().SendAsync(request);
        }

        #endregion

        #region Helper Method
        private static FormUrlEncodedContent MakeUrlEncodedContent(
            List<KeyValuePair<string, string>> parameters,
            List<KeyValuePair<string, string>> headers = null)
        {
            var formUrlEncodedContent = new FormUrlEncodedContent(parameters);
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    formUrlEncodedContent.Headers.Add(header.Key, header.Value);
                }
            }

            return formUrlEncodedContent;
        }
        #endregion
    }
}