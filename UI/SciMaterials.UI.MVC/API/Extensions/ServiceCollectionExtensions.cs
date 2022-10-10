using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using SciMaterials.UI.MVC.API.Configuration;
using SciMaterials.UI.MVC.API.Configuration.Interfaces;
using SciMaterials.UI.MVC.API.Helpers;
using SciMaterials.UI.MVC.API.Interfaces.Services;
using SciMaterials.UI.MVC.API.Services.Stores;
using SciMaterials.UI.MVC.Services;
using System.Reflection;

namespace SciMaterials.UI.MVC.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration config)
    {
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<FileUploadFilter>();
        });

        return services;
    }

    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration config)
    {
        var apiSettings = config.GetSection(ApiSettings.SectionName);
        services.Configure<ApiSettings>(apiSettings);
        services.AddSingleton<IApiSettings, ApiSettings>(s => s.GetRequiredService<IOptions<ApiSettings>>().Value);

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = apiSettings.GetValue<long>("MaxFileSize"); ;
        });

        return services;
    }
}