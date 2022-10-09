using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.SqlLite.Auth.Migrations;

class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, service) =>
            {
                var connectionString = hostContext.Configuration.GetConnectionString("AuthDbConnection");

                var dbConfig = hostContext.Configuration.GetSection("SqliteDbConfig");
        
                var builder = new SqliteConnectionStringBuilder(connectionString);
                builder.DataSource = dbConfig["datasource"];

                connectionString = builder.ConnectionString;
                
                service.AddDbContextFactory<AuthSqliteDbContext>(opt =>
                {
                    opt.UseSqlite(connectionString, x => 
                        x.MigrationsAssembly("SciMaterials.SqlLite.Auth.Migrations"));
                });
                service.AddHostedService<AuthWorker>();
            });
    }
}