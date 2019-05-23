using System;
using System.Net.Http;

namespace HttpRecorder.Repositories.HAR
{
    /// <summary>
    /// Describes details about response content
    /// https://w3c.github.io/web-performance/specs/HAR/Overview.html#content.
    /// </summary>
    public class Content
    {
        private const string EncodingBase64 = "base64";

        /// <summary>
        /// Initializes a new instance of the <see cref="Content"/> class.
        /// </summary>
        public Content()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Content"/> class from <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The <see cref="HttpContent"/> to initialize from.</param>
        public Content(HttpContent content)
        {
            if (content != null)
            {
                var bodyBytes = content.ReadAsByteArrayAsync().Result;
                Size = bodyBytes.Length;
                MimeType = content.Headers?.ContentType?.ToString();
                if (content.IsBinary())
                {
                    Text = Convert.ToBase64String(bodyBytes);
                    Encoding = EncodingBase64;
                }
                else
                {
                    Text = System.Text.Encoding.UTF8.GetString(bodyBytes);
                }
            }
        }

        /// <summary>
        /// Gets or sets the length of the returned content in bytes.
        /// </summary>
        public long Size { get; set; } = -1;

        /// <summary>
        /// Gets or sets the MIME type of the response text (value of the Content-Type response header).
        /// The charset attribute of the MIME type is included (if available).
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the response body sent from the server or loaded from the browser cache.
        /// This field is populated with textual content only. The text field is either HTTP decoded text
        /// or a encoded (e.g. "base64") representation of the response body. Leave out this field if the information is not available.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the encoding used for response text field e.g "base64".
        /// Leave out this field if the text field is HTTP decoded (decompressed and unchunked), than trans-coded from its original character set into UTF-8.
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// Returns a <see cref="ByteArrayContent"/>.
        /// </summary>
        /// <returns>Either <see cref="ByteArrayContent"/>, or null if no content.</returns>
        public ByteArrayContent ToHttpContent()
        {
            ByteArrayContent result = null;
            if (!string.IsNullOrEmpty(Text))
            {
                if (string.Equals(Encoding, EncodingBase64, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = new ByteArrayContent(Convert.FromBase64String(Text));
                }
                else
                {
                    result = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(Text));
                }
            }

            return result;
        }
    }
}
