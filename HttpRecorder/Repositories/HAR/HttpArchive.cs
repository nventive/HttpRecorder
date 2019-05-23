using System;
using System.Linq;

namespace HttpRecorder.Repositories.HAR
{
    /// <summary>
    /// Represents an HTTP Archive file content (https://w3c.github.io/web-performance/specs/HAR/Overview.html).
    /// </summary>
    public class HttpArchive
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpArchive"/> class.
        /// </summary>
        public HttpArchive()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpArchive"/> class from <paramref name="interaction"/>.
        /// </summary>
        /// <param name="interaction">The <see cref="Interaction"/> to use to initialize.</param>
        public HttpArchive(Interaction interaction)
        {
            foreach (var message in interaction.Messages)
            {
                Log.Entries.Add(new Entry(message));
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Log"/>.
        /// </summary>
        public Log Log { get; set; } = new Log();

        /// <summary>
        /// Returns an <see cref="Interaction"/>.
        /// </summary>
        /// <param name="interactionName">The <see cref="Interaction.Name"/>.</param>
        /// <returns>The interaction created from this.</returns>
        public Interaction ToInteraction(string interactionName)
        {
            return new Interaction(interactionName, Log.Entries.Select(x => x.ToInteractionMessage()));
        }
    }
}
