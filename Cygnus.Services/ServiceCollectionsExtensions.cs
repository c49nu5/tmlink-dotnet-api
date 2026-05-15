using Cygnus.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cygnus.Services.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Cygnus.Services;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddCygnusServices(this IServiceCollection services)
    {
        services.AddSingleton<IMeasurementConverter, MeasurementConverter>();
    }
}
