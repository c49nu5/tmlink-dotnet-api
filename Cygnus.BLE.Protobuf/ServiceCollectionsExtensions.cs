using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cygnus.BLE.Protobuf;
public static class ServiceCollectionsExtensions
{
    public static void AddProtobufServices(this IServiceCollection services)
    {
        services.AddSingleton<IProtobufMessageConverter, ProtobufMessageConverter>();
        services.AddTransient<Protobuf1Channel>();
        services.AddKeyedTransient<IProtobufChannel, Protobuf1Channel>((byte)1);
        services.AddSingleton((s) =>
        {
            return (Func<byte, IProtobufChannel?>)((r) => s.GetKeyedService<IProtobufChannel>(r));
        });
    }
}
