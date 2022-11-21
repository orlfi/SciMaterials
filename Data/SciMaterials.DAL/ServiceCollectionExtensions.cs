using Microsoft.Extensions.DependencyInjection;

using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;

namespace SciMaterials.DAL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        // use assembly scan
        services.AddScoped<IUnitOfWork<SciMaterialsContext>, SciMaterialsFilesUnitOfWork>();
        return services;
    }
}
