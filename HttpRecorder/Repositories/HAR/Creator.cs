namespace HttpRecorder.Repositories.HAR
{
    /// <summary>
    /// This object contains information about the log creator application
    /// https://w3c.github.io/web-performance/specs/HAR/Overview.html#sec-har-object-types-creator.
    /// </summary>
    public class Creator
    {
        /// <summary>
        /// Gets or sets the name of the application that created the log.
        /// </summary>
        public string Name { get; set; } = "HttpRecorder";

        /// <summary>
        /// Gets or sets the version number of the application that created the log.
        /// </summary>
        public string Version { get; set; } = typeof(Creator).Assembly.GetName().Version.ToString();
    }
}
