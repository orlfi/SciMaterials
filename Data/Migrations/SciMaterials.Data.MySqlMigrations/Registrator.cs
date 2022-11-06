using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SciMaterials.DAL.Contexts;

namespace SciMaterials.Data.MySqlMigrations
{
    public static class Registrator
    {
        public static IServiceCollection AddSciMaterialsContextMySql(this IServiceCollection services,
            string connectionString, int majorVersion = 8, int minorVersion = 0, int buildVersion = 30)
            => services.AddDbContext<SciMaterialsContext>(
                opt => opt.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(majorVersion, minorVersion, buildVersion)),
                    o => o.MigrationsAssembly(typeof(Registrator).Assembly.FullName)));
    }
}
