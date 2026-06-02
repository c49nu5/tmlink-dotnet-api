using Cygnus.TMLink.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Cygnus.BLE.InTheHand;
public static class ServiceCollectionsExtensions
{
    public static void AddInTheHandBLEServices(this IServiceCollection services)
    {
        services.AddSingleton<ITMLinkDeviceDiscoverer, TMLinkDeviceDiscoverer>();
    }
}
