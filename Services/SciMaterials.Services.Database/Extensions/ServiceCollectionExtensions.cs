using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.Database.Configuration;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.Data.MySqlMigrations;
using SciMaterials.MsSqlServerMigrations;
using SciMaterials.PostgresqlMigrations;
using SciMaterials.Services.Database.Services.DbInitialization;
using SciMaterials.SQLiteMigrations;

namespace SciMaterials.Services.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContextMultipleProviders(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection(key: "DbSettings")
            .Get<DbSettings>();

        var dbProvider = dbSettings.DbProvider;
        var connectionString = configuration.GetSection(key: "DbSettings")
            .GetConnectionString(dbProvider);
        var dbProviderName = dbProvider.Split(".", StringSplitOptions.RemoveEmptyEntries)[0];

        switch (dbProviderName.ToLower())
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
                services.AddSciMaterialsContextSQLite(connectionString);
                break;

            default:
                throw new Exception($"Unsupported provider: {dbProvider}");
        }

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services) =>
        services.AddTransient<IDbInitializer, DbInitializer>();
}