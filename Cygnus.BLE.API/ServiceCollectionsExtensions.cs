using Cygnus.BLE.API.Interfaces;
using Cygnus.BLE.API.Models;
using Cygnus.BLE.API.Services;
using Cygnus.BLE.Interfaces;
using Cygnus.BLE.InTheHand;
using Cygnus.BLE.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cygnus.BLE.API.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Cygnus.BLE.API;
public static class ServiceCollectionsExtensions
{
    [ExcludeFromCodeCoverage]
    public static void AddBleServices(this IServiceCollection services, bool withInTheHand = true)
    {
        services.AddSingleton<IConnectionService, ConnectionService>();
        services.AddTransient<IBLEGaugeInternal, BLEGauge>();
        services.AddSingleton<Func<IBLEGaugeInternal>>(s => s.GetRequiredService<IBLEGaugeInternal>);
        services.AddProtobufServices();
        if (withInTheHand)
        {
            services.AddInTheHandBleServices();
        }
    }
}
