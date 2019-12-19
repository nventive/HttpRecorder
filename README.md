# HttpRecorder

.NET HttpClient integration tests made easy.

HttpRecorder is an `HttpMessageHandler` that can record and replay HTTP interactions through the standard `HttpClient` . This allows the creation of HTTP integration tests that are fast, repeatable and reliable.

Interactions are recorded using the [HTTP Archive format standard](https://en.wikipedia.org/wiki/.har), so that they are easily manipulated by your favorite tool of choice.

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)
[![Build Status](https://dev.azure.com/nventive-public/nventive/_apis/build/status/nventive.HttpRecorder?branchName=master)](https://dev.azure.com/nventive-public/nventive/_build/latest?definitionId=3&branchName=master)
![Nuget](https://img.shields.io/nuget/v/HttpRecorder.svg)

## Getting Started

Install the package:

```
Install-Package HttpRecorder
```

Here is an example of an integration tests using **HttpRecorder** (the `HttpRecorderDelegatingHandler`):

```csharp
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HttpRecorder;
using Xunit;

namespace Sample
{
    public class SampleIntegrationTests
    {
        [Fact]
        public async Task ItShould()
        {
            // Initialize the HttpClient with the recorded file
            // stored in a fixture repository.
            var client = CreateHttpClient();

            // Performs HttpClient operations.
            // The interaction is recorded if there are no record,
            // or replayed if there are
            // (without actually hitting the target API).
            // Fixture is recorded in the SampleIntegrationTestsFixtures\ItShould.har file.
            var response = await client.GetAsync("api/user");

            // Performs assertions.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private HttpClient CreateHttpClient(
            [CallerMemberName] string testName = "",
            [CallerFilePath] string filePath = "")
        {
            // The location of the file where the interaction is recorded.
            // We use the C# CallerMemberName/CallerFilePath attributes to
            // automatically set an appropriate path based on the test case.
            var interactionFilePath = Path.Join(
                Path.GetDirectoryName(filePath),
                $"{Path.GetFileNameWithoutExtension(filePath)}Fixtures",
                testName);

            // Initialize the HttpClient with HttpRecorderDelegatingHandler, which
            // records and replays the interactions.
            // Do not forget to set the InnerHandler property.
            return new HttpClient(
                new HttpRecorderDelegatingHandler(interactionFilePath) { InnerHandler = new HttpClientHandler() })
            {
                BaseAddress = new Uri("https://reqres.in/"),
            };
        }
    }
}
```

## Features

### Record mode

The  `HttpRecorderDelegatingHandler` can be run in different modes:

- Auto: Default mode - replay the interactions if the recording exists, otherwise record it.
- Record: Always record the interaction, even if a record is present.
- Replay: Always replay the interaction, throw if there is no recording.
- Passthrough: Always passes the request/response down the line, without any interaction

Just use the appropriate mode in the `HttpRecorderDelegatingHandler`  constructor.

The mode can also be overridden using the environment variable `HTTP_RECORDER_MODE`.
If this is set to any valid `HttpRecorderMode` value, it will override the mode set in the code,
except if this mode is `HttpRecorderMode.Passthrough`.
This is useful when running in a CI environment and you want to make sure that no
request goes out and all interactions are properly committed to the codebase.

### Customize the matching behavior

By default, matching of the recorded requests is done by comparing the HTTP Method and complete Request URI. The first request that match is used and will not be returned again in the current run.

If needed, the matching behavior can be customized using the `RulesMatcher`:

```csharp
using HttpRecorder.Matchers;

// Will match requests once in order, without comparing requests.
var matcher = RulesMatcher.MatchOnce;

// Will match requests once only by comparing HTTP methods.
matcher = RulesMatcher.MatchOnce.ByHttpMethod();

// Will match requests multiple times by comparing HTTP methods,
// request uri (excluding the query string) and the X-API-Key header.
matcher = RulesMatcher.MatchMultiple
    .ByHttpMethod()
    .ByRequestUri(UriPartial.Path)
    .ByHeader("X-API-Key");

// Custom matching rule using the provided incoming request
// and a recorded interaction message.
matcher = RulesMatcher.MatchOnce.By((request, message) => ...);

var client = new HttpClient(new HttpRecorderDelegatingHandler("...", matcher: matcher));
```

Additional customization can be done by providing a custom `IRequestMatcher` implementation.

### Anonymize the records

Sometimes, there are portions of the requests / responses that you don't want recorded
(e.g. because of API keys you do not want to commit to the source code repo...).

In this case, you can use the `RulesInteractionAnonymizer` to perform the substitution.

```csharp
using HttpRecorder.Anonymizers;

var anonymizer = RulesInteractionAnonymizer.Default
    .AnonymizeRequestQueryStringParameter("queryStringParam")
    .AnonymizeRequestHeader("requestHeader");

var client = new HttpClient(new HttpRecorderDelegatingHandler("...", anonymizer: anonymizer));
```

Additional customization can be done by providing a custom `IInteractionAnonymizer`
implementation.

### Record interaction in external tools

Interaction files can be recorded using your favorite tool (e.g. [Fiddler](https://www.telerik.com/fiddler), Google Chrome Inspector, ...).

You only have to export it using the HAR/HTTP Archive format. They can then be used as-is as a test fixture that will be loaded by the `HttpRecorderDelegatingHandler`.

### Customize the storage

Reading/writing the interaction can be customized by providing a custom `IInteractionRepository` implementation.

## Changelog

Please consult the [CHANGELOG](CHANGELOG.md) for more information about version
history.

## License

This project is licensed under the Apache 2.0 license - see the
[LICENSE](LICENSE) file for details.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on the process for
contributing to this project.

Be mindful of our [Code of Conduct](CODE_OF_CONDUCT.md).

## Acknowledgments

- https://github.com/vcr/vcr
- https://github.com/nock/nock
- https://github.com/mleech/scotch
