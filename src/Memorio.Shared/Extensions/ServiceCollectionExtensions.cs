using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        return services;
    }
}
