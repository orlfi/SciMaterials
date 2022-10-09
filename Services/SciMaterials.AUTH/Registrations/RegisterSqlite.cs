using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.Auth.Registrations;

public static class RegisterSqlite
{
    public static IServiceCollection RegisterSqliteProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var connection_string = configuration.GetConnectionString("AuthDbConnection");

        var dbConfig = configuration.GetSection("SqliteDbConfig");

        var builder = new SqliteConnectionStringBuilder(connection_string);

        builder.DataSource = dbConfig["datasource"];


        connection_string = builder.ConnectionString;

        return services.AddDbContext<AuthSqliteDbContext>(options =>
            options.UseSqlite(connection_string));
    }
}