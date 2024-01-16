using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpRecorder
{
    /// <summary>
    /// <see cref="HttpResponseMessage"/> extension methods.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Returns a copy of the <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
        /// <returns>A new <see cref="HttpResponseMessage"/>.</returns>
        public static HttpResponseMessage Clone(this HttpResponseMessage response)
        {
            if (response == null)
            {
                return null;
            }

            var copiedResponse = new HttpResponseMessage
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = response.ReasonPhrase,
                RequestMessage = response.RequestMessage,
                Version = (Version)response?.Version.Clone(),
            };

            foreach (var header in response.Headers)
            {
                copiedResponse.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (response.Content == null)
            {
                return copiedResponse;
            }

            copiedResponse.Content = new ByteArrayContent(response.Content.ReadAsByteArrayAsync().Result);
            foreach (var header in response.Content.Headers)
            {
                copiedResponse.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return copiedResponse;
        }
    }
}
