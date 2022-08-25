using System;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using HttpRecorder.Matchers;
using Newtonsoft.Json;
using Xunit;

namespace HttpRecorder.Tests.Matchers
{
    public class RulesMatcherUnitTests
    {
        [Fact]
        public void ItShouldMatchOnce()
        {
            var interaction = BuildInteraction(
                new HttpRequestMessage { RequestUri = new Uri("http://first") },
                new HttpRequestMessage { RequestUri = new Uri("http://second") });
            var request = new HttpRequestMessage();

            var matcher = RulesMatcher.MatchOnce;

            var result = matcher.Match(request, interaction);

            result.Response.RequestMessage!.RequestUri.Should().BeEquivalentTo(new Uri("http://first"));

            result = matcher.Match(request, interaction);

            result.Should().NotBeNull();
            result.Response.RequestMessage!.RequestUri.Should().BeEquivalentTo(new Uri("http://second"));
        }

        [Fact]
        public void ItShouldMatchOnceByHttpMethod()
        {
            var interaction = BuildInteraction(
                new HttpRequestMessage(),
                new HttpRequestMessage { RequestUri = new Uri("http://first"), Method = HttpMethod.Get },
                new HttpRequestMessage { RequestUri = new Uri("http://second"), Method = HttpMethod.Head });
            var request = new HttpRequestMessage { Method = HttpMethod.Head };

            var matcher = RulesMatcher.MatchOnce
                .ByHttpMethod();

            var result = matcher.Match(request, interaction);

            result.Should().NotBeNull();
            result.Response.RequestMessage!.Method.Should().BeEquivalentTo(HttpMethod.Head);
        }

        [Fact]
        public void ItShouldMatchOnceByCompleteRequestUri()
        {
            var interaction = BuildInteraction(
                new HttpRequestMessage(),
                new HttpRequestMessage { RequestUri = new Uri("http://first?name=foo") },
                new HttpRequestMessage { RequestUri = new Uri("http://first?name=bar") });
            var request = new HttpRequestMessage { RequestUri = new Uri("http://first?name=bar") };

            var matcher = RulesMatcher.MatchOnce
                .ByRequestUri();

            var result = matcher.Match(request, interaction);

            result.Should().NotBeNull();
            result.Response.RequestMessage!.RequestUri.Should().BeEquivalentTo(new Uri("http://first?name=bar"));
        }

        [Fact]
        public void ItShouldMatchOnceByPartialRequestUri()
        {
            var interaction = BuildInteraction(
                new HttpRequestMessage(),
                new HttpRequestMessage { RequestUri = new Uri("http://first?name=foo") },
                new HttpRequestMessage { RequestUri = new Uri("http://first?name=bar") });
            var request = new HttpRequestMessage { RequestUri = new Uri("http://first?name=bar") };

            var matcher = RulesMatcher.MatchOnce
                .ByRequestUri(UriPartial.Path);

            var result = matcher.Match(request, interaction);

            result.Should().NotBeNull();
            result.Response.RequestMessage!.RequestUri.Should().BeEquivalentTo(new Uri("http://first?name=foo"));
        }

        [Fact]
        public void ItShouldMatchOnceByHeader()
        {
            var headerName = "If-None-Match";
            var firstRequest = new HttpRequestMessage();
            firstRequest.Headers.TryAddWithoutValidation(headerName, "first");
            var secondRequest = new HttpRequestMessage();
            secondRequest.Headers.TryAddWithoutValidation(headerName, "second");
            var interaction = BuildInteraction(new HttpRequestMessage(), firstRequest, secondRequest);
            var request = new HttpRequestMessage();
            request.Headers.TryAddWithoutValidation(headerName, "second");

            var matcher = RulesMatcher.MatchOnce
                .ByHeader(headerName);

            var result = matcher.Match(request, interaction);

            result.Should().NotBeNull();
            result.Response.RequestMessage!.Headers.IfNoneMatch.ToString().Should().Be("second");
        }

        [Fact]
        public void ItShouldMatchOnceByContentWithSameSize()
        {
            var firstContent = new ByteArrayContent(new byte[] { 0, 1, 2, 3 });
            var secondContent = new ByteArrayContent(new byte[] { 3, 2, 1, 0 });
            var interaction = BuildInteraction(
                new HttpRequestMessage(),
                new HttpRequestMessage { Content = firstContent },
                new HttpRequestMessage { Content = secondContent });
            var request = new HttpRequestMessage { Content = secondContent };

            var matcher = RulesMatcher.MatchOnce
                .ByContent();

            var result = matcher.Match(request, interaction);

            result.Should().NotBeNull();
            result.Response.RequestMessage!.Content.Should().BeEquivalentTo(secondContent);
        }

        [Fact]
        public void ItShouldMatchOnceByContentWithDifferentSizes()
        {
            var firstContent = new ByteArrayContent(new byte[] { 0, 1 });
            var secondContent = new ByteArrayContent(new byte[] { 3, 2, 1, 0 });
            var interaction = BuildInteraction(
                new HttpRequestMessage(),
                new HttpRequestMessage { Content = firstContent },
                new HttpRequestMessage { Content = secondContent });
            var request = new HttpRequestMessage { Content = secondContent };

            var matcher = RulesMatcher.MatchOnce
                .ByContent();

            var result = matcher.Match(request, interaction);

            result.Should().NotBeNull();
            result.Response.RequestMessage!.Content.Should().BeEquivalentTo(secondContent);
        }

        [Fact]
        public void ItShouldMatchOnceByJsonContent()
        {
            var firstModel = new Model { Name = "first" };
            var secondModel = new Model { Name = "second" };
            var firstContent = new StringContent(JsonConvert.SerializeObject(firstModel));
            var secondContent = new StringContent(JsonConvert.SerializeObject(secondModel));

            var interaction = BuildInteraction(
                new HttpRequestMessage(),
                new HttpRequestMessage { Content = firstContent },
                new HttpRequestMessage { Content = secondContent });
            var request = new HttpRequestMessage { Content = secondContent };

            var matcher = RulesMatcher.MatchOnce
                .ByJsonContent<Model>();

            var result = matcher.Match(request, interaction);

            result.Should().NotBeNull();
            result.Response.RequestMessage!.Content.Should().BeEquivalentTo(secondContent);
        }

        [Fact]
        public void ItShouldWorkWithNoMatch()
        {
            var interaction = BuildInteraction();
            var request = new HttpRequestMessage();

            var matcher = RulesMatcher.MatchOnce
                .ByHttpMethod();

            var result = matcher.Match(request, interaction);
            result.Should().BeNull();
        }

        [Fact]
        public void ItShouldMatchMultiple()
        {
            var interaction = BuildInteraction(
                new HttpRequestMessage());
            var request = new HttpRequestMessage();

            var matcher = RulesMatcher.MatchMultiple;

            var result = matcher.Match(request, interaction);
            result.Should().NotBeNull();

            result = matcher.Match(request, interaction);
            result.Should().NotBeNull();
        }

        [Fact]
        public void ItShouldMatchWithCombination()
        {
            var interaction = BuildInteraction(
                new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri("http://first") },
                new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri("http://second") });
            var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri("http://second") };

            var matcher = RulesMatcher.MatchOnce
                .ByHttpMethod()
                .ByRequestUri();

            var result = matcher.Match(request, interaction);
            result.Response.RequestMessage!.RequestUri.Should().BeEquivalentTo(new Uri("http://second"));
        }

        private static Interaction BuildInteraction(params HttpRequestMessage[] requests)
        {
            return new Interaction(
                "test",
                requests.Select(x => new InteractionMessage(
                    new HttpResponseMessage { RequestMessage = x },
                    new InteractionMessageTimings(DateTimeOffset.UtcNow, TimeSpan.MinValue))));
        }

        private class Model
        {
            public string Name { get; init; }

            public override bool Equals(object obj)
            {
                return Equals(obj as Model);
            }

            public bool Equals(Model other)
            {
                if (other == null)
                {
                    return false;
                }

                return string.Equals(Name, other.Name, StringComparison.InvariantCulture);
            }

            public override int GetHashCode() => Name == null ? 0 : Name.GetHashCode(StringComparison.InvariantCulture);
        }
    }
}
