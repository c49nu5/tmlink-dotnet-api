using Cygnus.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Cygnus.Services;
public static class ServiceCollectionsExtensions
{
    public static void AddCygnusServices(this IServiceCollection services)
    {
        services.AddSingleton<IMeasurementConverter, MeasurementConverter>();
    }
}
