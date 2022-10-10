using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using SciMaterials.Contracts.API.Settings;
using SciMaterials.Services.Configuration;
using SciMaterials.UI.MVC.API.Filters;

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