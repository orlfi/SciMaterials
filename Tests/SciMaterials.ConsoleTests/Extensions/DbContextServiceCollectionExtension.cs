using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SciMaterials.DAL.Contexts;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbContextServiceCollectionExtension
    {
        public static IServiceCollection AddContextMultipleProviders(this IServiceCollection services, HostBuilderContext context)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            
            var defaultProvider = context.Configuration["DbProvider"];

            services.AddDbContext<SciMaterialsContext>(options => _ = defaultProvider switch
            {
                "SqlServer" => options.UseSqlServer(context.Configuration.GetConnectionString("SqlServerConnectionString"), 
                    optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.MsSqlServerMigrations")),
                "PostgreSQL" => options.UseNpgsql(context.Configuration.GetConnectionString("PostgreSQLConnectionString"), 
                    optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.PostgresqlMigrations")),
                "MySQL" => options.UseMySql(context.Configuration.GetConnectionString("MySQLConnectionString"), new MySqlServerVersion(new Version(8, 0, 30)),
                    optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.Data.MySqlMigrations")),
                "SQLite" => options.UseSqlite(context.Configuration.GetConnectionString("SQLiteConnectionString"),
                    optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.SQLiteMigrations")),
                _ => throw new Exception($"Unsupported provider: {defaultProvider}")
            });

            return services;
        }
    }
}
