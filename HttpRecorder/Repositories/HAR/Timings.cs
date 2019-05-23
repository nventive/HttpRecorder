namespace HttpRecorder.Repositories.HAR
{
    /// <summary>
    /// Describes various phases within request-response round trip. All times are specified in milliseconds.
    /// https://w3c.github.io/web-performance/specs/HAR/Overview.html#timings.
    /// </summary>
    public class Timings
    {
        /// <summary>
        /// Gets or sets the time required to send HTTP request to the server.
        /// </summary>
        public double Send { get; set; }

        /// <summary>
        /// Gets or sets the waiting for a response from the server.
        /// </summary>
        public double Wait { get; set; }

        /// <summary>
        /// Gets or sets the time required to read entire response from the server (or cache).
        /// </summary>
        public double Receive { get; set; }
    }
}
