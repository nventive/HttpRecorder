using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace HttpRecorder.Anonymizers
{
    /// <summary>
    /// <see cref="IInteractionAnonymizer"/> that uses a rule system to conceal sensitive information.
    /// </summary>
    public sealed class RulesInteractionAnonymizer : IInteractionAnonymizer
    {
        /// <summary>
        /// The default value to use when replacing values.
        /// </summary>
        public const string DefaultAnonymizerReplaceValue = "******";

        private readonly IEnumerable<Action<InteractionMessage>> _rules;

        private RulesInteractionAnonymizer(
            IEnumerable<Action<InteractionMessage>> rules = null)
        {
            _rules = rules ?? Enumerable.Empty<Action<InteractionMessage>>();
        }

        /// <summary>
        /// Gets the default <see cref="RulesInteractionAnonymizer"/>.
        /// </summary>
        public static RulesInteractionAnonymizer Default { get; } = new RulesInteractionAnonymizer();

        /// <inheritdoc />
        async Task<Interaction> IInteractionAnonymizer.Anonymize(Interaction interaction, CancellationToken cancellationToken)
        {
            if (!_rules.Any())
            {
                return interaction;
            }

            if (interaction == null)
            {
                throw new ArgumentNullException(nameof(interaction));
            }

            return new Interaction(interaction.Name, interaction.Messages.Select(x =>
            {
                foreach (var rule in _rules)
                {
                    rule(x);
                }

                return x;
            }));
        }

        /// <summary>
        /// Add an anomizing rule.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        /// <returns>A new instance of <see cref="RulesInteractionAnonymizer"/> with the added rule.</returns>
        public RulesInteractionAnonymizer WithRule(Action<InteractionMessage> rule)
            => new RulesInteractionAnonymizer(_rules.Concat(new[] { rule }));

        /// <summary>
        /// Adds a rule that anonymize a request query parameter.
        /// </summary>
        /// <param name="parameterName">The query parameter name.</param>
        /// <param name="pattern">The replacement pattern. Defaults to <see cref="DefaultAnonymizerReplaceValue"/>.</param>
        /// <returns>A new instance of <see cref="RulesInteractionAnonymizer"/> with the added rule.</returns>
        public RulesInteractionAnonymizer AnonymizeRequestQueryStringParameter(string parameterName, string pattern = DefaultAnonymizerReplaceValue)
            => WithRule((x) =>
            {
                if (!string.IsNullOrEmpty(x.Response?.RequestMessage?.RequestUri?.Query))
                {
                    var queryString = HttpUtility.ParseQueryString(x.Response.RequestMessage.RequestUri.Query);
                    if (!string.IsNullOrEmpty(queryString[parameterName]))
                    {
                        queryString[parameterName] = pattern;
                        x.Response.RequestMessage.RequestUri = new Uri($"{x.Response.RequestMessage.RequestUri.GetLeftPart(UriPartial.Path)}?{queryString}");
                    }
                }
            });

        /// <summary>
        /// Adds a rule that anonymize a query parameter.
        /// </summary>
        /// <param name="headerName">The header name.</param>
        /// <param name="pattern">The replacement pattern. Defaults to <see cref="DefaultAnonymizerReplaceValue"/>.</param>
        /// <returns>A new instance of <see cref="RulesInteractionAnonymizer"/> with the added rule.</returns>
        public RulesInteractionAnonymizer AnonymizeRequestHeader(string headerName, string pattern = DefaultAnonymizerReplaceValue)
            => WithRule((x) =>
            {
                if (x.Response?.RequestMessage == null)
                {
                    return;
                }

                if (x.Response.RequestMessage.Headers.Contains(headerName))
                {
                    x.Response.RequestMessage.Headers.Remove(headerName);
                    x.Response.RequestMessage.Headers.TryAddWithoutValidation(headerName, pattern);
                }

                if (x.Response.RequestMessage.Content != null && x.Response.RequestMessage.Content.Headers.Contains(headerName))
                {
                    x.Response.RequestMessage.Content.Headers.Remove(headerName);
                    x.Response.RequestMessage.Content.Headers.TryAddWithoutValidation(headerName, pattern);
                }
            });
    }
}
