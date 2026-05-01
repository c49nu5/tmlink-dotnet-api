using Cygnus.BLE.API.Interfaces;
using Cygnus.BLE.API.Models;
using Cygnus.BLE.API.Services;
using Cygnus.BLE.Protobuf;
using Microsoft.Extensions.DependencyInjection;

namespace Cygnus.BLE.API;
public static class ServiceCollectionsExtensions
{
    public static void AddBleServices(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionService, ConnectionService>();
        services.AddSingleton<IGaugeDiscoverer, GaugeDiscoverer>();
        services.AddTransient<IBLEGauge, BLEGauge>();
        services.AddSingleton<Func<IBLEGauge>>(s => s.GetRequiredService<IBLEGauge>);
        services.AddProtobufServices();
    }
}
