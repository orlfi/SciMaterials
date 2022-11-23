using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

using SciMaterials.Contracts.API.Mapping;
using SciMaterials.Contracts.API.Services.Authors;
using SciMaterials.Contracts.API.Services.Categories;
using SciMaterials.Contracts.API.Services.Comments;
using SciMaterials.Contracts.API.Services.ContentTypes;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Contracts.API.Services.Tags;
using SciMaterials.Contracts.API.Services.Urls;
using SciMaterials.Contracts.API.Settings;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.Contracts.WebApi.Clients.Categories;
using SciMaterials.Contracts.WebApi.Clients.Comments;
using SciMaterials.Contracts.WebApi.Clients.ContentTypes;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.Contracts.WebApi.Clients.Tags;
using SciMaterials.DAL.Contracts.Configuration;
using SciMaterials.DAL.Contracts.Services;
using SciMaterials.Services.API.Services.Authors;
using SciMaterials.Services.API.Services.Categories;
using SciMaterials.Services.API.Services.Comments;
using SciMaterials.Services.API.Services.ContentTypes;
using SciMaterials.Services.API.Services.Files.Stores;
using SciMaterials.Services.API.Services.Files;
using SciMaterials.Services.API.Services.Tags;
using SciMaterials.Services.API.Services.Urls;
using SciMaterials.Services.ShortLinks;
using SciMaterials.UI.MVC.API.Filters;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Services;
using SciMaterials.DAL.Resources.UnitOfWork;
using SciMaterials.Data.MySqlMigrations;
using SciMaterials.MsSqlServerMigrations;
using SciMaterials.PostgresqlMigrations;
using SciMaterials.SQLiteMigrations;
using SciMaterials.WebApi.Clients.Categories;
using SciMaterials.WebApi.Clients.Comments;
using SciMaterials.WebApi.Clients.ContentTypes;
using SciMaterials.WebApi.Clients.Files;
using SciMaterials.WebApi.Clients.Tags;

namespace SciMaterials.UI.MVC;

public static class ResourcesRegister
{
    public static IServiceCollection AddApiClients(this IServiceCollection services, Uri apiAddress)
    {
        services.AddHttpClient<IFilesClient, FilesClient>("FilesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ICategoriesClient, CategoriesClient>("FilesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ICommentsClient, CommentsClient>("FilesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<IContentTypesClient, ContentTypesClient>("FilesClient", c => c.BaseAddress = apiAddress);
        services.AddHttpClient<ITagsClient, TagsClient>("FilesClient", c => c.BaseAddress = apiAddress);
        return services;
    }
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork<SciMaterialsContext>, SciMaterialsFilesUnitOfWork>();
        return services;
    }

    public static IServiceCollection AddDatabaseProviders(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection("DbSettings")
            .Get<DatabaseSettings>();

        var providerName = dbSettings.GetProviderName();
        var connectionString = configuration.GetSection("DbSettings").GetConnectionString(dbSettings.Provider);

        switch (providerName.ToLower())
        {
            case "sqlserver":
                services.AddSciMaterialsContextSqlServer(connectionString);
                break;
            case "postgresql":
                services.AddSciMaterialsContextPostgreSQL(connectionString);
                break;
            case "mysql":
                services.AddSciMaterialsContextMySql(connectionString);
                break;
            case "sqlite":
                services.AddSciMaterialsContextSqlite(connectionString);
                break;
            default:
                throw new Exception($"Unsupported provider: {providerName}");
        }

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services) =>
        services.AddTransient<IDatabaseManager, ResourcesDatabaseManager>();

    public static async Task InitializeDbAsync(this IApplicationBuilder app, IConfiguration configuration)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var db_setting = configuration.GetSection("DbSettings").Get<DatabaseSettings>();

        var manager = scope.ServiceProvider.GetRequiredService<ResourcesDatabaseManager>();

        if (db_setting.RemoveAtStart) await manager.DeleteDatabaseAsync();

        await manager.InitializeDatabaseAsync();

        if (db_setting.UseDataSeeder) await manager.SeedDatabaseAsync();
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileStore, FileSystemStore>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IContentTypeService, ContentTypeService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IUrlService, UrlService>();
        services.AddScoped<IUrlService, UrlService>();
        services.AddScoped<ILinkReplaceService, LinkReplaceService>();
        services.AddScoped<ILinkShortCutService, LinkShortCutService>();
        services.AddRepositoryServices();
        services.AddDatabaseServices();
        services.AddMappings();

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<FileUploadFilter>();
        });

        return services;
    }

    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration config)
    {
        var api_settings = config.GetSection(ApiSettings.SectionName);

        services
            .Configure<ApiSettings>(api_settings)
            .AddSingleton<ApiSettings>(s => s.GetRequiredService<IOptions<ApiSettings>>().Value);

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = api_settings.GetValue<long>("MaxFileSize");
        });

        return services;
    }
}