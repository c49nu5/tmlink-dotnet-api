using Cygnus.TMLink.API.Interfaces;
using Cygnus.TMLink.API.Models;
using Cygnus.TMLink.API.Services;
using Cygnus.BLE.InTheHand;
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
    public static void AddTMLinkServices(this IServiceCollection services, bool withInTheHandBLE = true)
    {
        services.AddSingleton<IConnectionService, ConnectionService>();
        services.AddTransient<ITMLinkGaugeInternal, TMLinkGauge>();
        services.AddSingleton<Func<ITMLinkGaugeInternal>>(s => s.GetRequiredService<ITMLinkGaugeInternal>);
        services.AddProtobufServices();
        if (withInTheHandBLE)
        {
            services.AddInTheHandBLEServices();
        }
    }
}
