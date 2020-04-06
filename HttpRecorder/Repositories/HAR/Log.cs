using System.Collections.Generic;

namespace HttpRecorder.Repositories.HAR
{
    /// <summary>
    /// This object represents the root of the exported data.
    /// https://w3c.github.io/web-performance/specs/HAR/Overview.html#sec-har-object-types-log.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Gets or sets the Version number of the format.
        /// </summary>
        public string Version { get; set; } = "1.2";

        /// <summary>
        /// Gets or sets the <see cref="Creator"/>.
        /// </summary>
        public Creator Creator { get; set; } = new Creator();

        /// <summary>
        /// Gets or sets the list of <see cref="Entry"/>.
        /// </summary>
        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}
