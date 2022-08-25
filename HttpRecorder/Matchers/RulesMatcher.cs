using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace HttpRecorder.Matchers
{
    /// <summary>
    /// <see cref="IRequestMatcher"/> implementation that matches <see cref="HttpRequestMessage"/>
    /// Additional rules can be specified.
    /// </summary>
    public class RulesMatcher : IRequestMatcher
    {
        private readonly IEnumerable<Func<HttpRequestMessage, InteractionMessage, bool>> _rules;
        private readonly bool _matchOnce;
        private readonly IList<InteractionMessage> _matchedInteractionMessages = new List<InteractionMessage>();

        private RulesMatcher(IEnumerable<Func<HttpRequestMessage, InteractionMessage, bool>> rules = null, bool matchOnce = true)
        {
            _rules = rules ?? Enumerable.Empty<Func<HttpRequestMessage, InteractionMessage, bool>>();
            _matchOnce = matchOnce;
        }

        /// <summary>
        /// Gets a new <see cref="RulesMatcher"/> that matches request in sequence and only once.
        /// </summary>
        public static RulesMatcher MatchOnce { get => new RulesMatcher(Enumerable.Empty<Func<HttpRequestMessage, InteractionMessage, bool>>(), true); }

        /// <summary>
        /// Gets a new <see cref="RulesMatcher"/> that matches request in sequence and multiple times.
        /// </summary>
        public static RulesMatcher MatchMultiple { get => new RulesMatcher(Enumerable.Empty<Func<HttpRequestMessage, InteractionMessage, bool>>(), false); }

        /// <inheritdoc />
        public InteractionMessage Match(HttpRequestMessage request, Interaction interaction)
        {
            if (interaction == null)
            {
                throw new ArgumentNullException(nameof(interaction));
            }

            IEnumerable<InteractionMessage> query = interaction.Messages;

            if (_matchOnce)
            {
                query = query.Where(x => !_matchedInteractionMessages.Contains(x));
            }

            foreach (var rule in _rules)
            {
                query = query.Where(x => rule(request, x));
            }

            var matchedInteraction = query.FirstOrDefault();

            if (matchedInteraction != null && _matchOnce)
            {
                _matchedInteractionMessages.Add(matchedInteraction);
            }

            return matchedInteraction;
        }

        /// <summary>
        /// Returns a new <see cref="RulesMatcher"/> with the added <paramref name="rule"/>.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        /// <returns>A new <see cref="RulesMatcher"/>.</returns>
        public RulesMatcher By(Func<HttpRequestMessage, InteractionMessage, bool> rule)
            => new RulesMatcher(_rules.Concat(new[] { rule }), _matchOnce);

        /// <summary>
        /// Adds a rule that matches by <see cref="HttpMethod"/>.
        /// </summary>
        /// <returns>A new <see cref="RulesMatcher"/>.</returns>
        public RulesMatcher ByHttpMethod()
            => By((request, message) => request.Method == message.Response.RequestMessage.Method);

        /// <summary>
        /// Adds a rule that matches by <see cref="HttpRequestMessage.RequestUri"/>.
        /// </summary>
        /// <param name="part">Specify a <see cref="UriPartial"/> to restrict the matching to a subset of the request <see cref="Uri"/>.</param>
        /// <returns>A new <see cref="RulesMatcher"/>.</returns>
        public RulesMatcher ByRequestUri(UriPartial part = UriPartial.Query)
            => By((request, message) => string.Equals(request.RequestUri?.GetLeftPart(part), message.Response.RequestMessage.RequestUri?.GetLeftPart(part), StringComparison.InvariantCulture));

        /// <summary>
        /// Adds a rule that matches by comparing request header values.
        /// </summary>
        /// <param name="headerName">The name of the header to compare values from.</param>
        /// <param name="stringComparison">Allows customization of the string comparison.</param>
        /// <returns>A new <see cref="RulesMatcher"/>.</returns>
        public RulesMatcher ByHeader(string headerName, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
            => By((request, message) =>
            {
                string requestHeader = null;
                string interactionHeader = null;

                if (request.Headers.TryGetValues(headerName, out var requestValues))
                {
                    requestHeader = string.Join(",", requestValues);
                }

                if (request.Content != null && request.Content.Headers.TryGetValues(headerName, out var requestContentValues))
                {
                    requestHeader = string.Join(",", requestContentValues);
                }

                if (message.Response.RequestMessage.Headers.TryGetValues(headerName, out var interactionValues))
                {
                    interactionHeader = string.Join(",", interactionValues);
                }

                if (message.Response.RequestMessage.Content != null && message.Response.RequestMessage.Content.Headers.TryGetValues(headerName, out var interactionContentValues))
                {
                    interactionHeader = string.Join(",", interactionContentValues);
                }

                return string.Equals(requestHeader, interactionHeader, stringComparison);
            });

        /// <summary>
        /// Adds a rule that matches by binary comparing the <see cref="HttpRequestMessage.Content"/>.
        /// </summary>
        /// <returns>A new <see cref="RulesMatcher"/>.</returns>
        public RulesMatcher ByContent()
            => By((request, message) =>
            {
                var requestContent = request.Content?.ReadAsByteArrayAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                var messageContent = message.Response.RequestMessage.Content?.ReadAsByteArrayAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                if (requestContent is null)
                {
                    return messageContent is null;
                }
                else
                {
                    if (messageContent is null)
                    {
                        return false;
                    }
                }

                if (requestContent.Length != messageContent.Length)
                {
                    return false;
                }

                return StructuralComparisons.StructuralComparer.Compare(
                      request.Content?.ReadAsByteArrayAsync().Result,
                      message.Response.RequestMessage.Content?.ReadAsByteArrayAsync().Result) == 0;
            });

        /// <summary>
        /// Adds a rule that matches by comparing the JSON content of the requests.
        /// </summary>
        /// <typeparam name="T">The json object type.</typeparam>
        /// <param name="equalityComparer"><see cref="IEqualityComparer{T}"/> to use. Defaults to <see cref="EqualityComparer{T}.Default"/>.</param>
        /// <param name="jsonSerializerOptions">The <see cref="JsonSerializerOptions"/> to use.</param>
        /// <returns>A new <see cref="RulesMatcher"/>.</returns>
        public RulesMatcher ByJsonContent<T>(
            IEqualityComparer<T> equalityComparer = null,
            JsonSerializerOptions jsonSerializerOptions = null)
            => By((request, message) =>
            {
                var requestContent = request.Content?.ReadAsStringAsync().Result;
                var requestJson = !string.IsNullOrEmpty(requestContent) ? JsonSerializer.Deserialize<T>(requestContent, jsonSerializerOptions) : default(T);

                var interactionContent = message.Response.RequestMessage.Content?.ReadAsStringAsync().Result;
                var interactionJson = !string.IsNullOrEmpty(interactionContent) ? JsonSerializer.Deserialize<T>(interactionContent, jsonSerializerOptions) : default(T);

                if (equalityComparer == null)
                {
                    equalityComparer = EqualityComparer<T>.Default;
                }

                return equalityComparer.Equals(requestJson, interactionJson);
            });
    }
}
