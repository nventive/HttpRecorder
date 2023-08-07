using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HttpRecorder.Anonymizers;
using Xunit;

namespace HttpRecorder.Tests.Anonymizers
{
    public class RulesInteractionAnonymizerTests
    {
        [Fact]
        public async Task ItShouldDoNothingByDefault()
        {
            var interaction = BuildInteraction(
                new HttpRequestMessage { RequestUri = new Uri("http://first") });

            IInteractionAnonymizer anonymizer = RulesInteractionAnonymizer.Default;

            var result = await anonymizer.Anonymize(interaction);
            result.Should().BeEquivalentTo(interaction);
        }

        [Fact]
        public async Task ItShouldAnonymizeRequestQueryStringParameter()
        {
            var interaction = BuildInteraction(
                new HttpRequestMessage { RequestUri = new Uri("http://first/") },
                new HttpRequestMessage { RequestUri = new Uri("https://second/?key=foo&value=bar") });

            IInteractionAnonymizer anonymizer = RulesInteractionAnonymizer.Default
                .AnonymizeRequestQueryStringParameter("key");

            var result = await anonymizer.Anonymize(interaction);
            result.Messages[0].Response.RequestMessage!.RequestUri!.ToString().Should().Be("http://first/");
            result.Messages[1].Response.RequestMessage!.RequestUri!.ToString().Should().Be($"https://second/?key={RulesInteractionAnonymizer.DefaultAnonymizerReplaceValue}&value=bar");
        }

        [Fact]
        public async Task ItShouldAnonymizeRequestHeader()
        {
            var request = new HttpRequestMessage();
            request.Headers.TryAddWithoutValidation("X-RequestHeader", "Value");
            request.Content = new ByteArrayContent(Array.Empty<byte>());
            request.Content.Headers.TryAddWithoutValidation("X-RequestHeader", "Value2");
            var interaction = BuildInteraction(request);

            IInteractionAnonymizer anonymizer = RulesInteractionAnonymizer.Default
                .AnonymizeRequestHeader("X-RequestHeader");

            var result = await anonymizer.Anonymize(interaction);
            result.Messages[0].Response.RequestMessage!.Headers.GetValues("X-RequestHeader").First().Should().Be(RulesInteractionAnonymizer.DefaultAnonymizerReplaceValue);
        }

        private static Interaction BuildInteraction(params HttpRequestMessage[] requests)
        {
            return new Interaction(
                "test",
                requests.Select(x => new InteractionMessage(
                    new HttpResponseMessage { RequestMessage = x },
                    new InteractionMessageTimings(DateTimeOffset.UtcNow, TimeSpan.MinValue))));
        }
    }
}
