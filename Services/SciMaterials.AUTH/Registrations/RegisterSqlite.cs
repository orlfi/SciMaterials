using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.Auth.Registrations;

public static class RegisterSqlite
{
    public static IServiceCollection RegisterSqliteProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AuthDbConnection");

        //var dbConfig = configuration.GetSection("SqliteDbConfig");
        
        var builder = new SqliteConnectionStringBuilder(connectionString);
        builder.DataSource = 

        connectionString = builder.ConnectionString;
        
        return services.AddDbContext<AuthSqliteDbContext>(options =>
            options.UseSqlite(connectionString));
    }
}