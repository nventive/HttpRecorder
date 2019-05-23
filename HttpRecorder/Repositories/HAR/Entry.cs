using System;

namespace HttpRecorder.Repositories.HAR
{
    /// <summary>
    /// Represents an exported HTTP requests.
    /// https://w3c.github.io/web-performance/specs/HAR/Overview.html#entries.
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> class.
        /// </summary>
        public Entry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> class from <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The <see cref="InteractionMessage"/> to initialize from.</param>
        public Entry(InteractionMessage message)
        {
            StartedDateTime = message.Timings.StartedDateTime;
            Time = Convert.ToInt64(Math.Round(message.Timings.Time.TotalMilliseconds, 0));
            Request = new Request(message.Response.RequestMessage);
            Response = new Response(message.Response);
        }

        /// <summary>
        /// Gets or sets the date and time stamp of the request start.
        /// </summary>
        public DateTimeOffset StartedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the total elapsed time of the request in milliseconds.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Request"/>.
        /// </summary>
        public Request Request { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Response"/>.
        /// </summary>
        public Response Response { get; set; }

        /// <summary>
        /// Gets or sets info about cache usage. NOT SUPPORTED.
        /// </summary>
        public object Cache { get; set; } = new object();

        /// <summary>
        /// Gets or sets the <see cref="Timings"/>.
        /// </summary>
        public Timings Timings { get; set; } = new Timings();

        /// <summary>
        /// Returns a <see cref="InteractionMessage"/>.
        /// </summary>
        /// <returns>The <see cref="InteractionMessage"/> created from this.</returns>
        public InteractionMessage ToInteractionMessage()
        {
            var request = Request.ToHttpRequestMessage();
            var response = Response.ToHttpResponseMessage();
            response.RequestMessage = request;

            return new InteractionMessage(
                response,
                new InteractionMessageTimings(StartedDateTime, TimeSpan.FromMilliseconds(Time)));
        }
    }
}
