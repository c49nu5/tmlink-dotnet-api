using Cygnus.BLE.API.Services;
using Cygnus.BLE.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Cygnus.BLE.InTheHand;
public static class ServiceCollectionsExtensions
{
    public static void AddInTheHandBleServices(this IServiceCollection services)
    {
        services.AddSingleton<IGaugeDiscoverer, GaugeDiscoverer>();
    }
}
