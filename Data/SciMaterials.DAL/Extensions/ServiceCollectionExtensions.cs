using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.InitializationDb.Implementation;
using SciMaterials.DAL.InitializationDb.Interfaces;

namespace SciMaterials.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContextMultipleProviders(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            
        var defaultProvider = configuration["DbProvider"];

        services.AddDbContext<SciMaterialsContext>(options => _ = defaultProvider switch
        {
            "SqlServer" => options.UseSqlServer(configuration.GetConnectionString("SqlServerConnectionString"), 
                optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.MsSqlServerMigrations")),
            "PostgreSQL" => options.UseNpgsql(configuration.GetConnectionString("PostgreSQLConnectionString"), 
                optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.PostgresqlMigrations")),
            "MySQL" => options.UseMySql(configuration.GetConnectionString("MySQLConnectionString"), new MySqlServerVersion(new Version(8, 0, 30)),
                optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.Data.MySqlMigrations")),
            "SQLite" => options.UseSqlite(configuration.GetConnectionString("SQLiteConnectionString"),
                optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.SQLiteMigrations")),
            _ => throw new Exception($"Unsupported provider: {defaultProvider}")
        });

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        services.AddTransient<IDbInitializer, DbInitializer>();
        return services;
    }
}