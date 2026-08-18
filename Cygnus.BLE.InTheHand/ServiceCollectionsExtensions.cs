using Cygnus.TMLink.Interfaces;
using InTheHand.Bluetooth;
using Microsoft.Extensions.DependencyInjection;

namespace Cygnus.BLE.InTheHand;
public static class ServiceCollectionsExtensions
{
    public static void AddInTheHandBLEServices(this IServiceCollection services)
    {
        services.AddTransient<BLECharacteristic>();
        services.AddSingleton<Func<GattCharacteristic, BLECharacteristic>>(s => c => ActivatorUtilities.CreateInstance<BLECharacteristic>(s, c));

        services.AddTransient<BLEDevice>();
        services.AddSingleton<Func<BluetoothDevice, ITMLinkDevice>>(s => d => ActivatorUtilities.CreateInstance<BLEDevice>(s, d));

        services.AddSingleton<ITMLinkDeviceDiscoverer, TMLinkDeviceDiscoverer>();
    }
}
