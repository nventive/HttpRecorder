using System.Collections.Generic;
using System.Linq;

namespace HttpRecorder
{
    /// <summary>
    /// An interaction is a complete recording of a set of <see cref="InteractionMessage"/>.
    /// </summary>
    public class Interaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Interaction"/> class.
        /// </summary>
        /// <param name="name">The interaction name.</param>
        /// <param name="messages">The set of <see cref="InteractionMessage"/>.</param>
        public Interaction(
            string name,
            IEnumerable<InteractionMessage> messages = null)
        {
            Name = name;
            Messages = (messages ?? Enumerable.Empty<InteractionMessage>()).ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the interaction name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="InteractionMessage"/> list.
        /// </summary>
        public IReadOnlyList<InteractionMessage> Messages { get; }
    }
}
