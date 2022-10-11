using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SciMaterials.Contracts.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}