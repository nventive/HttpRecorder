using System;
using System.Net.Http;

namespace HttpRecorder
{
    /// <summary>
    /// Represents a single HTTP Interaction (Request/Response).
    /// <see cref="HttpRequestMessage"/> is in the <see cref="HttpResponseMessage.RequestMessage"/> property.
    /// </summary>
    public class InteractionMessage
    {
        private HttpResponseMessage _response;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionMessage"/> class.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
        /// <param name="timings">The <see cref="InteractionMessageTimings"/>.</param>
        public InteractionMessage(
            HttpResponseMessage response,
            InteractionMessageTimings timings)
        {
            _response = response;
            Timings = timings;
        }

        /// <summary>
        /// Gets the <see cref="HttpResponseMessage"/>.
        /// </summary>
        public HttpResponseMessage Response { get => _response.Clone(); }

        /// <summary>
        /// Gets the <see cref="InteractionMessageTimings"/>.
        /// </summary>
        public InteractionMessageTimings Timings { get; }
    }
}
