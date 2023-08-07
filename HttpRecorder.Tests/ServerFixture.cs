using System;
using System.Linq;
using HttpRecorder.Tests.Server;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace HttpRecorder.Tests
{
    /// <summary>
    /// xUnit collection fixture that starts an ASP.NET Core server listening to a random port.
    /// <seealso cref="ServerCollection" />.
    /// </summary>
    public class ServerFixture : IDisposable
    {
        public ServerFixture()
        {
            ServerWebHost = WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseUrls("http://127.0.0.1:0")
                .Build();
            ServerWebHost.Start();
        }

        public IWebHost ServerWebHost { get; }

        public Uri ServerUri
        {
            get
            {
                var serverAddressesFeature = ServerWebHost.ServerFeatures.Get<IServerAddressesFeature>();
                return new Uri(serverAddressesFeature!.Addresses.First());
            }
        }

        public void Dispose()
        {
            if (ServerWebHost != null)
            {
                ServerWebHost.Dispose();
            }
        }
    }
}
