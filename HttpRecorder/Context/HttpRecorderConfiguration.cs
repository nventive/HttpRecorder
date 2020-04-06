using System.Net.Http;
using HttpRecorder.Anonymizers;
using HttpRecorder.Matchers;
using HttpRecorder.Repositories;
using HttpRecorder.Repositories.HAR;

namespace HttpRecorder.Context
{
    /// <summary>
    /// Specific configuration for a <see cref="HttpRecorderContext"/>.
    /// </summary>
    public class HttpRecorderConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether recording is enabled.
        /// Defaults to true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the name of the interaction.
        /// If you use the default <see cref="IInteractionRepository"/>, this will be the path to the HAR file (relative or absolute) and
        /// if no file extension is provided, .har will be used.
        /// </summary>
        public string InteractionName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="HttpRecorderMode" />. Defaults to <see cref="HttpRecorderMode.Auto" />.
        /// </summary>
        public HttpRecorderMode Mode { get; set; } = HttpRecorderMode.Auto;

        /// <summary>
        /// Gets or sets the <see cref="IRequestMatcher"/> to use to match interactions with incoming <see cref="HttpRequestMessage"/>.
        /// Defaults to matching Once by <see cref="HttpMethod"/> and <see cref="HttpRequestMessage.RequestUri"/>.
        /// <see cref="RulesMatcher.ByHttpMethod"/> and <see cref="RulesMatcher.ByRequestUri"/>.
        /// </summary>
        public IRequestMatcher Matcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IInteractionRepository"/> to use to read/write the interaction.
        /// Defaults to <see cref="HttpArchiveInteractionRepository"/>.
        /// </summary>
        public IInteractionRepository Repository { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IInteractionAnonymizer"/> to use to anonymize the interaction.
        /// Defaults to <see cref="RulesInteractionAnonymizer.Default"/>.
        /// </summary>
        public IInteractionAnonymizer Anonymizer { get; set; }
    }
}
