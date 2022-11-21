using Microsoft.Extensions.DependencyInjection;

using SciMaterials.DAL.UnitOfWork;

namespace SciMaterials.DAL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        return services;
    }
}
