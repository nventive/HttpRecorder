using System.Net.Http;
using System.Text.RegularExpressions;

namespace HttpRecorder
{
    /// <summary>
    /// <see cref="HttpContent"/> extension methods.
    /// </summary>
    public static class HttpContentExtensions
    {
        private static readonly Regex BinaryMimeRegex = new Regex("(image/*|audio/*|video/*|application/octet-stream|multipart/form-data)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex FormDataMimeRegex = new Regex("application/x-www-form-urlencoded", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /// <summary>
        /// Indicates whether <paramref name="content"/> represents binary content.
        /// </summary>
        /// <param name="content">The <see cref="HttpContent"/>.</param>
        /// <returns>true if it is binary, false otherwise.</returns>
        public static bool IsBinary(this HttpContent content)
        {
            var contentType = content?.Headers?.ContentType?.MediaType;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            return BinaryMimeRegex.IsMatch(contentType);
        }

        /// <summary>
        /// Indicates whether <paramref name="content"/> represents form data (URL-encoded) content.
        /// </summary>
        /// <param name="content">The <see cref="HttpContent"/>.</param>
        /// <returns>true if it is form data, false otherwise.</returns>
        public static bool IsFormData(this HttpContent content)
        {
            var contentType = content?.Headers?.ContentType?.MediaType;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            return FormDataMimeRegex.IsMatch(contentType);
        }
    }
}
