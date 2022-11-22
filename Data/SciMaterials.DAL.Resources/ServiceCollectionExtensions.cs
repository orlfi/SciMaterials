using Microsoft.Extensions.DependencyInjection;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.UnitOfWork;

namespace SciMaterials.DAL.Resources;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        // use assembly scan
        services.AddScoped<IUnitOfWork<SciMaterialsContext>, SciMaterialsFilesUnitOfWork>();
        return services;
    }
}
