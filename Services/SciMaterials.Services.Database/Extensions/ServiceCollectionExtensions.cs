using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.Database.Configuration;
using SciMaterials.Contracts.Database.Enums;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.DAL.Contexts;
using SciMaterials.Services.Database.Enums;
using SciMaterials.Services.Database.Services.DbInitialization;

namespace SciMaterials.Services.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContextMultipleProviders(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch(switchName: "Npgsql.EnableLegacyTimestampBehavior", isEnabled: true);

        var dbSettings = configuration.GetSection(key: "DbSettings")
            .Get<DbSettings>();

        var providerName = dbSettings.GetProviderName();
        var connectionString = dbSettings.GetConnectionString();

        services.AddDbContext<SciMaterialsContext>(options => _ = providerName switch
        {
            nameof(DbProviderNames.SqlServer) => GetOptionsSqlServer(options, connectionString),
            nameof(DbProviderNames.PostgreSQL) => GetOptionsPostgreSQL(options, connectionString),
            nameof(DbProviderNames.MySQL) => GetOptionsMySQL(options, connectionString),
            nameof(DbProviderNames.SQLite) => GetOptionsSQLite(options, connectionString),
            _ => throw new Exception($"Unsupported provider: {providerName}")
        });

        return services;
    }

    private static DbContextOptionsBuilder GetOptionsSqlServer(DbContextOptionsBuilder options, string connectionString)
    {
        var builder = options.UseSqlServer(connectionString,
            opt => opt.MigrationsAssembly(Migrations.SqlServer.ToDescriptionString()));
        return builder;
    }

    private static DbContextOptionsBuilder GetOptionsPostgreSQL(DbContextOptionsBuilder options, string connectionString)
    {
        var builder = options.UseNpgsql(connectionString,
            opt => opt.MigrationsAssembly(Migrations.PostgreSQL.ToDescriptionString()));
        return builder;
    }

    private static DbContextOptionsBuilder GetOptionsMySQL(DbContextOptionsBuilder options, string connectionString)
    {
        var builder = options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 30)),
            opt => opt.MigrationsAssembly(Migrations.MySQL.ToDescriptionString()));
        return builder;
    }

    private static DbContextOptionsBuilder GetOptionsSQLite(DbContextOptionsBuilder options, string connectionString)
    {
        var builder = options.UseSqlite(connectionString,
            opt => opt.MigrationsAssembly(Migrations.SQLite.ToDescriptionString()));
        return builder;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services) =>
        services.AddTransient<IDbInitializer, DbInitializer>();
}