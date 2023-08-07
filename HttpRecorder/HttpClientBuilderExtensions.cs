using System.Net.Http;
using HttpRecorder;
using HttpRecorder.Anonymizers;
using HttpRecorder.Matchers;
using HttpRecorder.Repositories;
using HttpRecorder.Repositories.HAR;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IHttpClientBuilder"/> extension methods.
    /// </summary>
    public static class HttpClientBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="HttpRecorderDelegatingHandler"/> as a HttpMessageHandler in the client pipeline.
        /// </summary>
        /// <param name="httpClientBuilder">The <see cref="IHttpClientBuilder"/>.</param>
        /// <param name="interactionName">
        /// The name of the interaction.
        /// If you use the default <see cref="IInteractionRepository"/>, this will be the path to the HAR file (relative or absolute) and
        /// if no file extension is provided, .har will be used.
        /// </param>
        /// <param name="mode">The <see cref="HttpRecorderMode" />. Defaults to <see cref="HttpRecorderMode.Auto" />.</param>
        /// <param name="matcher">
        /// The <see cref="IRequestMatcher"/> to use to match interactions with incoming <see cref="HttpRequestMessage"/>.
        /// Defaults to matching Once by <see cref="HttpMethod"/> and <see cref="HttpRequestMessage.RequestUri"/>.
        /// <see cref="RulesMatcher.ByHttpMethod"/> and <see cref="RulesMatcher.ByRequestUri"/>.
        /// </param>
        /// <param name="repository">
        /// The <see cref="IInteractionRepository"/> to use to read/write the interaction.
        /// Defaults to <see cref="HttpArchiveInteractionRepository"/>.
        /// </param>
        /// <param name="anonymizer">
        /// The <see cref="IInteractionAnonymizer"/> to use to anonymize the interaction.
        /// Defaults to <see cref="RulesInteractionAnonymizer.Default"/>.
        /// </param>
        /// <returns>The updated <see cref="IHttpClientBuilder"/>.</returns>
        public static IHttpClientBuilder AddHttpRecorder(
            this IHttpClientBuilder httpClientBuilder,
            string interactionName,
            HttpRecorderMode mode = HttpRecorderMode.Auto,
            IRequestMatcher matcher = null,
            IInteractionRepository repository = null,
            IInteractionAnonymizer anonymizer = null)
        {
            var recorder = new HttpRecorderDelegatingHandler(
                    interactionName,
                    mode: mode,
                    matcher: matcher,
                    repository: repository,
                    anonymizer: anonymizer);

            return httpClientBuilder.AddHttpMessageHandler(_ => recorder);
}
    }
}
