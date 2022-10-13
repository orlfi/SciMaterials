using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.MySql.Auth.Migrations;

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

                var dbConfig = hostContext.Configuration.GetSection("DbConfig");
        
                var builder = new MySqlConnectionStringBuilder(connectionString);
                builder.Server = dbConfig["server"];
                builder.Database = dbConfig["database"];
                builder.Port = Convert.ToUInt32(dbConfig["port"]);
                builder.UserID = dbConfig["userid"];
                builder.Password = dbConfig["password"];

                connectionString = builder.ConnectionString;
                
                service.AddDbContextFactory<AuthMySqlDbContext>(opt =>
                {
                    opt.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 30)), 
                        x => x.MigrationsAssembly("SciMaterials.MySql.Auth.Migrations"));
                });
                service.AddHostedService<AuthWorker>();
            });
    }
}