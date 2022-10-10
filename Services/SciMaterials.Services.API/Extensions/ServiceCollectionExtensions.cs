using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Services.API.Services.Files.Stores;
using System.Reflection;

namespace SciMaterials.Services.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IFileStore, FileSystemStore>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }

    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}