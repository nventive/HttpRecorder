using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HttpRecorder.Anonymizers;
using HttpRecorder.Repositories;
using HttpRecorder.Tests.Server;
using Moq;
using Xunit;

namespace HttpRecorder.Tests
{
    /// <summary>
    /// <see cref="HttpRecorderDelegatingHandler"/> integration tests.
    /// We do exclude the response Date headers from comparison as not to get skewed
    /// by timing issues.
    /// </summary>
    [Collection(ServerCollection.Name)]
    public class HttpRecorderIntegrationTests
    {
        private readonly ServerFixture _fixture;

        public HttpRecorderIntegrationTests(ServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ItShouldGetJson()
        {
            HttpResponseMessage passthroughResponse = null;

            await ExecuteModeIterations(async (client, mode) =>
            {
                var response = await client.GetAsync(ApiController.JsonUri);

                response.EnsureSuccessStatusCode();
                response.Headers.Remove("Date");
                if (mode == HttpRecorderMode.Passthrough)
                {
                    passthroughResponse = response;
                    var result = await response.Content.ReadAsAsync<SampleModel>();
                    result.Name.Should().Be(SampleModel.DefaultName);
                }
                else
                {
                    response.Should().BeEquivalentTo(passthroughResponse);
                }
            });
        }

        [Fact]
        public async Task ItShouldGetJsonWithQueryString()
        {
            HttpResponseMessage passthroughResponse = null;
            var name = "Bar";

            await ExecuteModeIterations(async (client, mode) =>
            {
                var response = await client.GetAsync($"{ApiController.JsonUri}?name={name}");

                response.EnsureSuccessStatusCode();
                response.Headers.Remove("Date");
                if (mode == HttpRecorderMode.Passthrough)
                {
                    passthroughResponse = response;
                    var result = await response.Content.ReadAsAsync<SampleModel>();
                    result.Name.Should().Be(name);
                }
                else
                {
                    response.Should().BeEquivalentTo(passthroughResponse);
                }
            });
        }

        [Fact]
        public async Task ItShouldPostJson()
        {
            var sampleModel = new SampleModel();
            HttpResponseMessage passthroughResponse = null;

            await ExecuteModeIterations(async (client, mode) =>
            {
                var response = await client.PostAsJsonAsync(ApiController.JsonUri, sampleModel);

                response.EnsureSuccessStatusCode();
                response.Headers.Remove("Date");

                if (mode == HttpRecorderMode.Passthrough)
                {
                    passthroughResponse = response;
                    var result = await response.Content.ReadAsAsync<SampleModel>();
                    result.Name.Should().Be(sampleModel.Name);
                }
                else
                {
                    response.Should().BeEquivalentTo(passthroughResponse);
                }
            });
        }

        [Fact]
        public async Task ItShouldPostFormData()
        {
            var sampleModel = new SampleModel();
            HttpResponseMessage passthroughResponse = null;

            await ExecuteModeIterations(async (client, mode) =>
            {
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("name", sampleModel.Name),
                });

                var response = await client.PostAsync(ApiController.FormDataUri, formContent);

                response.EnsureSuccessStatusCode();
                response.Headers.Remove("Date");
                if (mode == HttpRecorderMode.Passthrough)
                {
                    passthroughResponse = response;
                    var result = await response.Content.ReadAsAsync<SampleModel>();
                    result.Name.Should().Be(sampleModel.Name);
                }
                else
                {
                    response.Should().BeEquivalentTo(passthroughResponse);
                }
            });
        }

        [Fact]
        public async Task ItShouldExecuteMultipleRequestsInParallel()
        {
            const int Concurrency = 10;
            IList<HttpResponseMessage> passthroughResponses = null;

            await ExecuteModeIterations(async (client, mode) =>
            {
                var tasks = new List<Task<HttpResponseMessage>>();

                for (var i = 0; i < Concurrency; i++)
                {
                    tasks.Add(client.GetAsync($"{ApiController.JsonUri}?name={i}"));
                }

                var responses = await Task.WhenAll(tasks);
                foreach (var response in responses)
                {
                    response.Headers.Remove("Date");
                }

                if (mode == HttpRecorderMode.Passthrough)
                {
                    passthroughResponses = responses;
                    for (var i = 0; i < Concurrency; i++)
                    {
                        var response = responses[i];
                        response.EnsureSuccessStatusCode();
                        var result = await response.Content.ReadAsAsync<SampleModel>();
                        result.Name.Should().Be($"{i}");
                    }
                }
                else
                {
                    responses.Should().BeEquivalentTo(passthroughResponses);
                }
            });
        }

        [Fact]
        public async Task ItShouldExecuteMultipleRequestsInSequenceWithRecorderModeAuto()
        {
            // Let's clean the record first if any.
            var recordedFileName = $"{nameof(ItShouldExecuteMultipleRequestsInSequenceWithRecorderModeAuto)}.har";
            if (File.Exists(recordedFileName))
            {
                File.Delete(recordedFileName);
            }

            var client = CreateHttpClient(
                HttpRecorderMode.Auto,
                nameof(ItShouldExecuteMultipleRequestsInSequenceWithRecorderModeAuto));
            var response1 = await client.GetAsync($"{ApiController.JsonUri}?name=1");
            var response2 = await client.GetAsync($"{ApiController.JsonUri}?name=2");
            var result1 = await response1.Content.ReadAsAsync<SampleModel>();
            result1.Name.Should().Be("1");

            var result2 = await response2.Content.ReadAsAsync<SampleModel>();
            result2.Name.Should().Be("2");

            // We resolve to replay at this point.
            client = CreateHttpClient(
               HttpRecorderMode.Auto,
               nameof(ItShouldExecuteMultipleRequestsInSequenceWithRecorderModeAuto));
            var response2_1 = await client.GetAsync($"{ApiController.JsonUri}?name=1");
            var response2_2 = await client.GetAsync($"{ApiController.JsonUri}?name=2");

            response2_1.Should().BeEquivalentTo(response1);
            response2_2.Should().BeEquivalentTo(response2);
        }

        [Fact]
        public async Task ItShouldGetBinary()
        {
            HttpResponseMessage passthroughResponse = null;
            var expectedBinaryContent = await File.ReadAllBytesAsync(typeof(ApiController).Assembly.Location);

            await ExecuteModeIterations(async (client, mode) =>
            {
                var response = await client.GetAsync(ApiController.BinaryUri);

                response.EnsureSuccessStatusCode();
                response.Headers.Remove("Date");

                if (mode == HttpRecorderMode.Passthrough)
                {
                    passthroughResponse = response;
                    var result = await response.Content.ReadAsByteArrayAsync();
                    result.Should().BeEquivalentTo(expectedBinaryContent);
                }
                else
                {
                    response.Should().BeEquivalentTo(passthroughResponse);
                }
            });
        }

        [Fact]
        public async Task ItShouldThrowIfDoesNotFindFile()
        {
            const string TestFile = "unknown.file";
            var client = CreateHttpClient(HttpRecorderMode.Replay, TestFile);

            Func<Task> act = async () => await client.GetAsync(ApiController.JsonUri);

            await act.Should().ThrowAsync<HttpRecorderException>()
                .WithMessage($"*{TestFile}*");
        }

        [Fact]
        public async Task ItShouldThrowIfFileIsCorrupted()
        {
            var file = typeof(HttpRecorderIntegrationTests).Assembly.Location;
            var client = CreateHttpClient(HttpRecorderMode.Replay, file);

            Func<Task> act = async () => await client.GetAsync(ApiController.JsonUri);

            await act.Should().ThrowAsync<HttpRecorderException>()
                .WithMessage($"*{file}*");
        }

        [Fact]
        public async Task ItShouldThrowIfNoRequestCanBeMatched()
        {
            var repositoryMock = new Mock<IInteractionRepository>();
            repositoryMock.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            repositoryMock.Setup(x => x.LoadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns<string, CancellationToken>((interactionName, _) => Task.FromResult(new Interaction(interactionName)));

            var client = CreateHttpClient(HttpRecorderMode.Replay, repository: repositoryMock.Object);

            Func<Task> act = async () => await client.GetAsync(ApiController.JsonUri);

            await act.Should().ThrowAsync<HttpRecorderException>()
                .WithMessage($"*{ApiController.JsonUri}*");
        }

        [Theory]
        [InlineData(202)]
        [InlineData(301)]
        [InlineData(303)]
        [InlineData(404)]
        [InlineData(500)]
        [InlineData(502)]
        public async Task ItShouldGetStatus(int statusCode)
        {
            HttpResponseMessage passthroughResponse = null;

            await ExecuteModeIterations(async (client, mode) =>
            {
                var response = await client.GetAsync($"{ApiController.StatusCodeUri}?statusCode={statusCode}");
                response.StatusCode.Should().Be((HttpStatusCode)statusCode);
                response.Headers.Remove("Date");

                if (mode == HttpRecorderMode.Passthrough)
                {
                    passthroughResponse = response;
                }
                else
                {
                    response.Should().BeEquivalentTo(passthroughResponse);
                }
            });
        }

        [Fact]
        public async Task ItShouldOverrideModeWithEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable(HttpRecorderDelegatingHandler.OverridingEnvironmentVariableName, HttpRecorderMode.Replay.ToString());
            try
            {
                var client = CreateHttpClient(HttpRecorderMode.Record);

                Func<Task> act = () => client.GetAsync(ApiController.JsonUri);

                await act.Should().ThrowAsync<HttpRecorderException>();
            }
            finally
            {
                Environment.SetEnvironmentVariable(HttpRecorderDelegatingHandler.OverridingEnvironmentVariableName, string.Empty);
            }
        }

        [Fact]
        public async Task ItShouldAnonymize()
        {
            var repositoryMock = new Mock<IInteractionRepository>();
            var client = CreateHttpClient(
                HttpRecorderMode.Record,
                repository: repositoryMock.Object,
                anonymizer: RulesInteractionAnonymizer.Default.AnonymizeRequestQueryStringParameter("key"));
            Func<Task> act = async () => await client.GetAsync($"{ApiController.JsonUri}?key=foo");
            await act.Should().ThrowAsync<InvalidOperationException>(); // Because we don't act on the stream in the repository. That's fine.

            repositoryMock.Verify(
                x => x.StoreAsync(
                    It.Is<Interaction>(i => i.Messages[0].Response.RequestMessage.RequestUri.ToString().EndsWith($"{ApiController.JsonUri}?key={RulesInteractionAnonymizer.DefaultAnonymizerReplaceValue}", StringComparison.Ordinal)),
                    It.IsAny<CancellationToken>()));
        }

        private async Task ExecuteModeIterations(Func<HttpClient, HttpRecorderMode, Task> test, [CallerMemberName] string testName = "")
        {
            var iterations = new[]
            {
                HttpRecorderMode.Passthrough,
                HttpRecorderMode.Record,
                HttpRecorderMode.Replay,
                HttpRecorderMode.Auto,
            };
            foreach (var mode in iterations)
            {
                var client = CreateHttpClient(mode, testName);
                await test(client, mode);
            }
        }

        private HttpClient CreateHttpClient(
            HttpRecorderMode mode,
            [CallerMemberName] string testName = "",
            IInteractionRepository repository = null,
            IInteractionAnonymizer anonymizer = null)
            => new HttpClient(
                new HttpRecorderDelegatingHandler(testName, mode: mode, repository: repository, anonymizer: anonymizer)
                {
                    InnerHandler = new HttpClientHandler(),
                })
            {
                BaseAddress = _fixture.ServerUri,
            };
    }
}
