using System.Collections.Generic;

namespace HttpRecorder.Repositories.HAR
{
    /// <summary>
    /// HTTP Header definition.
    /// https://w3c.github.io/web-performance/specs/HAR/Overview.html#headers.
    /// </summary>
    public class Header : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Header"/> class.
        /// </summary>
        public Header()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Header"/> class from <paramref name="keyValuePair"/>.
        /// </summary>
        /// <param name="keyValuePair">The <see cref="KeyValuePair{TKey, TValue}"/> to initialize from.</param>
        public Header(KeyValuePair<string, IEnumerable<string>> keyValuePair)
            : base(keyValuePair)
        {
        }
    }
}
