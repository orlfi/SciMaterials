#region usings
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.InitializationDb.Implementation;
using SciMaterials.DAL.InitializationDb.Interfaces;
#endregion

static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
    .ConfigureServices(ConfigureServices);


static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    var defaultProvider = context.Configuration.GetValue<string>("DefaultProvider");
    services.AddDbContext<SciMaterialsContext>(options => _ = defaultProvider switch
    {
        "SqlServer" => options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"),
            x => x.MigrationsAssembly("SciMaterials.MsSqlServerMigrations")),
        "PostgreSql" => options.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=SciMaterials;Integrated Security=True",
                x => x.MigrationsAssembly("SciMaterials.MsSqlServerMigrations")),
        _ => throw new Exception("Не задана БД по умолчанию")
    });
    services.AddTransient<IDbInitializer, DbInitializer>();
}

using IHost host = CreateHostBuilder(args).Build();

await using (var scope = host.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await db.InitializeDbAsync();
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();