using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SciMaterials.DAL.AUTH.Contracts;
using SciMaterials.DAL.Contracts.Configuration;
using SciMaterials.DAL.Contracts.Initialization;
using SciMaterials.DAL.Resources.Contexts;

namespace SciMaterials.Services.Database.Extensions;

public static class ApplicationExtension
{
    public static async Task InitializeDbAsync(this IApplicationBuilder app, IConfiguration configuration)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<SciMaterialsContext>();
        
        if (context.Database.IsSqlite())
            await context.Database.MigrateAsync().ConfigureAwait(false);

        var authDb = scope.ServiceProvider.GetRequiredService<IAuthDbInitializer>();
        await authDb.InitializeAsync();

        var db_setting = configuration.GetSection("DbSettings").Get<DatabaseSettings>();

        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        await initializer.InitializeDatabaseAsync(
                RemoveAtStart: db_setting.RemoveAtStart,
                UseDataSeeder: db_setting.UseDataSeeder);
    }
}