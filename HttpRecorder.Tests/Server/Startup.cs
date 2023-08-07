using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HttpRecorder.Tests.Server
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Default ASP.Net Core startup class.")]
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o => o.EnableEndpointRouting = false);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}
