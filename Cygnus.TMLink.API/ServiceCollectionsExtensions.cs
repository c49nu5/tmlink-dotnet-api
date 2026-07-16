using Cygnus.BLE.InTheHand;
using Cygnus.Interfaces;
using Cygnus.TMLink.API.Interfaces;
using Cygnus.TMLink.API.Models;
using Cygnus.TMLink.API.Services;
using Cygnus.TMLink.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cygnus.TMLink.API.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Cygnus.TMLink.API;
public static class ServiceCollectionsExtensions
{
    [ExcludeFromCodeCoverage]
    public static void AddTMLinkAPIServices(this IServiceCollection services, bool withInTheHandBLE = true)
    {
        services.AddSingleton<ITMLinkConnectionService, ConnectionService>();
        services.AddSingleton<IConnectionService>(s => s.GetRequiredService<ITMLinkConnectionService>());

        services.AddTransient<ITMLinkGauge, TMLinkGauge>();
        services.AddSingleton<Func<ITMLinkGauge>>(s => s.GetRequiredService<ITMLinkGauge>);
        services.AddProtobufServices();
        if (withInTheHandBLE)
        {
            services.AddInTheHandBLEServices();
        }
    }
}
