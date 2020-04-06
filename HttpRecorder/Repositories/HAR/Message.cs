using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace HttpRecorder.Repositories.HAR
{
    /// <summary>
    /// Base class for HAR messages.
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// Prefix to use for <see cref="HttpVersion"/>.
        /// </summary>
        protected const string HTTPVERSIONPREFIX = "HTTP/";

        /// <summary>
        /// Gets or sets the HTTP version.
        /// </summary>
        public string HttpVersion { get; set; }

        /// <summary>
        /// Gets or sets the list of cookie objects. NOT SUPPORTED.
        /// </summary>
        public List<object> Cookies { get; set; } = new List<object>();

        /// <summary>
        /// Gets or sets the list of <see cref="Header"/>.
        /// </summary>
        public List<Header> Headers { get; set; } = new List<Header>();

        /// <summary>
        /// Gets or sets the total number of bytes from the start of the HTTP request message until (and including) the double CRLF before the body.
        /// Set to -1 if the info is not available.
        /// </summary>
        public int HeadersSize { get; set; } = -1;

        /// <summary>
        /// Gets or sets the size of the request body (POST data payload) in bytes.
        /// Set to -1 if the info is not available.
        /// </summary>
        public int BodySize { get; set; } = -1;

        /// <summary>
        /// Returns a <see cref="Version"/> from <see cref="HttpVersion"/>;.
        /// </summary>
        /// <returns>The <see cref="Version"/>.</returns>
        protected Version GetVersion()
        {
            if (string.IsNullOrEmpty(HttpVersion))
            {
                return new Version();
            }

            var version = HttpVersion;
            if (version.StartsWith(HTTPVERSIONPREFIX, StringComparison.InvariantCultureIgnoreCase))
            {
                version = version.Substring(HTTPVERSIONPREFIX.Length);
            }

            return new Version(version);
        }

        /// <summary>
        /// Adds <see cref="Headers"/> tp <paramref name="headers"/>, without validation.
        /// <paramref name="headers"/> can be null.
        /// </summary>
        /// <param name="headers">The <see cref="HttpHeaders"/> to add to.</param>
        protected void AddHeadersWithoutValidation(HttpHeaders headers)
        {
            if (headers != null)
            {
                foreach (var header in Headers)
                {
                    if (!headers.TryGetValues(header.Name, out var _))
                    {
                        headers.TryAddWithoutValidation(header.Name, header.Value);
                    }
                }
            }
        }
    }
}
