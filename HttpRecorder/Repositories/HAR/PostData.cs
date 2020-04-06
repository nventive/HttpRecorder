using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace HttpRecorder.Repositories.HAR
{
    /// <summary>
    /// Describes posted data.
    /// https://w3c.github.io/web-performance/specs/HAR/Overview.html#postData.
    /// </summary>
    public class PostData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostData"/> class.
        /// </summary>
        public PostData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostData"/> class from <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The <see cref="HttpContent"/> to initialize from.</param>
        public PostData(HttpContent content)
        {
            if (content != null)
            {
                MimeType = content.Headers?.ContentType?.ToString();
                if (content.IsFormData())
                {
                    var bodyParams = HttpUtility.ParseQueryString(content.ReadAsStringAsync().Result);
                    foreach (string key in bodyParams)
                    {
                        Params.Add(new PostedParam { Name = key, Value = bodyParams[key] });
                    }
                }
                else
                {
                    Text = Encoding.UTF8.GetString(content.ReadAsByteArrayAsync().Result);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mime type of posted data.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="PostedParam"/>.
        /// </summary>
        public List<PostedParam> Params { get; set; } = new List<PostedParam>();

        /// <summary>
        /// Gets or sets plain text posted data.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Returns a <see cref="HttpContent"/>.
        /// </summary>
        /// <returns>Either <see cref="ByteArrayContent"/>, <see cref="FormUrlEncodedContent"/>, or null if no content.</returns>
        public HttpContent ToHttpContent()
        {
            HttpContent result = null;
            if (!string.IsNullOrEmpty(Text))
            {
                result = new ByteArrayContent(Encoding.UTF8.GetBytes(Text));
            }

            if (Params != null && Params.Count > 0)
            {
                result = new FormUrlEncodedContent(Params.Select(x => new KeyValuePair<string, string>(x.Name, x.Value)));
            }

            return result;
        }
    }
}
