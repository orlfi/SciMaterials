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
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

            var defaultProvider = context.Configuration.GetValue("Provider", "SqlServer");

            services.AddDbContext<SciMaterialsContext>(options => _ = defaultProvider switch
            {
                "SqlServer" => options.UseSqlServer(context.Configuration.GetConnectionString("SqlServerConnectionString"), 
                    optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.MsSqlServerMigrations")),
                "PostgreSQL" => options.UseNpgsql(context.Configuration.GetConnectionString("PostgreSQLConnectionString"), 
                    optionsBuilder => optionsBuilder.MigrationsAssembly("SciMaterials.PostgresqlMigrations")),
                _ => throw new Exception($"Unsupported provider: {defaultProvider}")
            });

            return services;
        }
    }
}
