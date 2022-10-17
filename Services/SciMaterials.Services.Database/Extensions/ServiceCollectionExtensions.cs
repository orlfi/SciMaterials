using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.Database.Configuration;
using SciMaterials.Contracts.Database.Enums;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.DAL.Contexts;
using SciMaterials.Services.Database.Services.DbInitialization;

namespace SciMaterials.Services.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContextMultipleProviders(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var dbSettings = configuration.GetSection("DbSettings")
            .Get<DbSettings>();

        services.AddDbContext<SciMaterialsContext>(options => _ = dbSettings.DbProvider switch
        {
            nameof(DbProviders.SqlServer) => options.UseSqlServer(configuration.GetConnectionString("SqlServerConnectionString"),
                optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.MsSqlServerMigrations")),
            nameof(DbProviders.PostgreSQL) => options.UseNpgsql(configuration.GetConnectionString("PostgreSQLConnectionString"),
                optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.PostgresqlMigrations")),
            nameof(DbProviders.MySQL) => options.UseMySql(configuration.GetConnectionString("MySQLConnectionString"), new MySqlServerVersion(new Version(8, 0, 30)),
                optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.Data.MySqlMigrations")),
            nameof(DbProviders.SQLite) => options.UseSqlite(configuration.GetConnectionString("SQLiteConnectionString"),
                optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.SQLiteMigrations")),
            _ => throw new Exception($"Unsupported provider: {dbSettings.DbProvider}")
        });

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services) =>
        services.AddTransient<IDbInitializer, DbInitializer>()
        ;
}