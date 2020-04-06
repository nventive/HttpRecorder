using HttpRecorder.Context;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods.
    /// </summary>
    public static class HttpRecorderServiceCollectionExtensions
    {
        /// <summary>
        /// Enables support for the <see cref="HttpRecorderContext"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddHttpRecorderContextSupport(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, RecorderHttpMessageHandlerBuilderFilter>());

            return services;
        }
    }
}
