using System.Collections.Generic;

namespace HttpRecorder.Repositories.HAR
{
    /// <summary>
    /// Base class for HTTP Archive name/value parameters.
    /// </summary>
    public abstract class Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        public Parameter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class from <paramref name="keyValuePair"/>.
        /// </summary>
        /// <param name="keyValuePair">The <see cref="KeyValuePair{TKey, TValue}"/> to initialize from.</param>
        protected Parameter(KeyValuePair<string, IEnumerable<string>> keyValuePair)
        {
            Name = keyValuePair.Key;
            Value = string.Join(",", keyValuePair.Value);
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }
    }
}
