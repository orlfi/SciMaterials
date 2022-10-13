using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.API.Extensions;
using SciMaterials.Contracts.API.Services.Categories;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Data.Extensions;
using SciMaterials.Domain.Extensions;
using SciMaterials.Services.API.Services.Categories;
using SciMaterials.Services.API.Services.Files;
using SciMaterials.Services.API.Services.Files.Stores;

namespace SciMaterials.Services.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileStore, FileSystemStore>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddRepositoryServices();
        services.AddContextMultipleProviders(configuration);
        services.AddDatabaseServices();
        services.AddMappings();
        return services;
    }
}