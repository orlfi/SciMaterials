using Microsoft.Extensions.DependencyInjection;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Data.UnitOfWork;

namespace SciMaterials.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        return services;
    }
}
