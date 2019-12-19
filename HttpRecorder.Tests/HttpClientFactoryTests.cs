using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HttpRecorder.Tests.Server;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HttpRecorder.Tests
{
    [Collection(ServerCollection.Name)]
    public class HttpClientFactoryTests
    {
        private readonly ServerFixture _fixture;

        public HttpClientFactoryTests(ServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ItShouldWorkWithHttpClientFactory()
        {
            var services = new ServiceCollection();
            services
                .AddHttpClient(
                    "TheClient",
                    options =>
                    {
                        options.BaseAddress = _fixture.ServerUri;
                    })
                .AddHttpRecorder(nameof(ItShouldWorkWithHttpClientFactory), HttpRecorderMode.Record);

            var client = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient("TheClient");
            var response = await client.GetAsync(ApiController.JsonUri);
            response.EnsureSuccessStatusCode();
        }
    }
}
