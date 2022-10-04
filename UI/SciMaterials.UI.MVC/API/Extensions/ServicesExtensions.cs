using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using SciMaterials.UI.MVC.API.Configuration;
using SciMaterials.UI.MVC.API.Configuration.Interfaces;
using SciMaterials.UI.MVC.API.Data;
using SciMaterials.UI.MVC.API.Data.Interfaces;
using SciMaterials.UI.MVC.API.Helpers;
using SciMaterials.UI.MVC.API.Services.Interfaces;
using SciMaterials.UI.MVC.API.Services.Stores;
using SciMaterials.UI.MVC.Services;

namespace SciMaterials.UI.MVC.API.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddApiSwagger(this IServiceCollection services, IConfiguration config)
    {
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<FileUploadFilter>();
        });
        
        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IFileService<Guid>, FileService>();
        services.AddScoped<IFileStore, FileSystemStore>();
        services.AddSingleton<IFileRepository<Guid>, FileInfoMemoryRepository>();

        return services;
    }

    public static IServiceCollection ConfigureApiServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ApiSettings>(config.GetSection(ApiSettings.SectionName));
        services.AddSingleton<IApiSettings, ApiSettings>(s => s.GetRequiredService<IOptions<ApiSettings>>().Value);

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = config.GetValue<long>("MaxFileSize"); ;
        });

        return services;
    }
}