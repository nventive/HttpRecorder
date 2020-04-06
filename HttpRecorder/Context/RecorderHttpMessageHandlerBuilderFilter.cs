using System;
using System.IO;
using Microsoft.Extensions.Http;

namespace HttpRecorder.Context
{
    /// <summary>
    /// <see cref="IHttpMessageHandlerBuilderFilter"/> that adds <see cref="HttpRecorderDelegatingHandler"/>
    /// based on the value of <see cref="HttpRecorderContext.Current"/>.
    /// </summary>
    public class RecorderHttpMessageHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecorderHttpMessageHandlerBuilderFilter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
        public RecorderHttpMessageHandlerBuilderFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc />
        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return (builder) =>
            {
                // Run other configuration first, we want to decorate.
                next(builder);

                var context = HttpRecorderContext.Current;
                if (context is null)
                {
                    return;
                }

                var config = context.ConfigurationFactory?.Invoke(_serviceProvider, builder) ?? new HttpRecorderConfiguration();

                if (config.Enabled)
                {
                    var interactionName = config.InteractionName;
                    if (string.IsNullOrEmpty(interactionName))
                    {
                        interactionName = Path.Combine(
                            Path.GetDirectoryName(context.FilePath),
                            $"{Path.GetFileNameWithoutExtension(context.FilePath)}Fixtures",
                            context.TestName,
                            builder.Name);
                    }

                    builder.AdditionalHandlers.Add(new HttpRecorderDelegatingHandler(
                        interactionName,
                        mode: config.Mode,
                        matcher: config.Matcher,
                        repository: config.Repository,
                        anonymizer: config.Anonymizer));
                }
            };
        }
    }
}
