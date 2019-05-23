using System.Net.Http;

namespace HttpRecorder
{
    /// <summary>
    /// The execution mode for <see cref="HttpRecorderDelegatingHandler" />.
    /// </summary>
    public enum HttpRecorderMode
    {
        /// <summary>
        /// Default mode.
        /// Uses <see cref="Replay" /> if a record is present, or <see cref="Record" /> if not.
        /// </summary>
        Auto,

        /// <summary>
        /// Always record the interaction, even if a record is present.
        /// Overrides previous record.
        /// </summary>
        Record,

        /// <summary>
        /// Always replay the interaction.
        /// Throws <see cref="HttpRecorderException" /> during execution if there is no record available.
        /// </summary>
        Replay,

        /// <summary>
        /// Always invoke the underlying <see cref="HttpMessageHandler" />, and do not record the interaction.
        /// Does not try to deserialize the message as well.
        /// </summary>
        Passthrough,
    }
}
