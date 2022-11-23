using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SciMaterials.DAL.Contracts.Configuration;
using SciMaterials.DAL.Contracts.Services;
using SciMaterials.DAL.Resources.Services;
using SciMaterials.Data.MySqlMigrations;
using SciMaterials.MsSqlServerMigrations;
using SciMaterials.PostgresqlMigrations;
using SciMaterials.SQLiteMigrations;

namespace SciMaterials.Services.Database.Extensions;

public static class ServiceCollectionExtensions
{
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
}