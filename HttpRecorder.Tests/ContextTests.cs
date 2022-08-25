using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HttpRecorder.Context;
using HttpRecorder.Tests.Server;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HttpRecorder.Tests
{
    [Collection(ServerCollection.Name)]
    public class ContextTests
    {
        private readonly ServerFixture _fixture;

        public ContextTests(ServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ItShouldWorkWithHttpRecorderContext()
        {
            var services = new ServiceCollection();
            services
                .AddHttpRecorderContextSupport()
                .AddHttpClient(
                    "TheClient",
                    options =>
                    {
                        options.BaseAddress = _fixture.ServerUri;
                    });

            HttpResponseMessage passthroughResponse;
            using (new HttpRecorderContext((_, _) => new HttpRecorderConfiguration
            {
                Mode = HttpRecorderMode.Record,
                InteractionName = nameof(ItShouldWorkWithHttpRecorderContext),
            }))
            {
                var client = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient("TheClient");
                passthroughResponse = await client.GetAsync(ApiController.JsonUri);
                passthroughResponse.EnsureSuccessStatusCode();
            }

            using (new HttpRecorderContext((_, _) => new HttpRecorderConfiguration
            {
                Mode = HttpRecorderMode.Replay,
                InteractionName = nameof(ItShouldWorkWithHttpRecorderContext),
            }))
            {
                var client = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient("TheClient");
                var response = await client.GetAsync(ApiController.JsonUri);
                response.EnsureSuccessStatusCode();
                response.Should().BeEquivalentTo(passthroughResponse);
            }
        }

        [Fact]
        public async Task ItShouldWorkWithHttpRecorderContextWhenNotRecording()
        {
            var services = new ServiceCollection();
            services
                .AddHttpRecorderContextSupport()
                .AddHttpClient(
                    "TheClient",
                    options =>
                    {
                        options.BaseAddress = _fixture.ServerUri;
                    });

            HttpResponseMessage passthroughResponse;
            using (new HttpRecorderContext((_, _) => new HttpRecorderConfiguration
            {
                Enabled = false,
                Mode = HttpRecorderMode.Record,
                InteractionName = nameof(ItShouldWorkWithHttpRecorderContextWhenNotRecording),
            }))
            {
                var client = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient("TheClient");
                passthroughResponse = await client.GetAsync(ApiController.JsonUri);
                passthroughResponse.EnsureSuccessStatusCode();
            }

            using (new HttpRecorderContext((_, _) => new HttpRecorderConfiguration
            {
                Mode = HttpRecorderMode.Replay,
                InteractionName = nameof(ItShouldWorkWithHttpRecorderContextWhenNotRecording),
            }))
            {
                var client = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient("TheClient");
                Func<Task> act = async () => await client.GetAsync(ApiController.JsonUri);
                await act.Should().ThrowAsync<HttpRecorderException>();
            }
        }

        [Fact]
        public void ItShouldNotAllowMultipleContexts()
        {
            using var context = new HttpRecorderContext();
            Action act = () => { _ = new HttpRecorderContext(); };
            act.Should().Throw<HttpRecorderException>().WithMessage("*multiple*");
        }
    }
}
