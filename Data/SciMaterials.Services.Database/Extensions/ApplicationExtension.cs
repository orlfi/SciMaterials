using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SciMaterials.DAL.Contracts.Configuration;
using SciMaterials.DAL.Resources.Services;

namespace SciMaterials.Services.Database.Extensions;

public static class ApplicationExtension
{
    public static async Task InitializeDbAsync(this IApplicationBuilder app, IConfiguration configuration)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var db_setting = configuration.GetSection("DbSettings").Get<DatabaseSettings>();

        var manager = scope.ServiceProvider.GetRequiredService<ResourcesDatabaseManager>();

        if (db_setting.RemoveAtStart) await manager.DeleteDatabaseAsync();

        await manager.InitializeDatabaseAsync();

        if (db_setting.UseDataSeeder) await manager.SeedDatabaseAsync();
    }
}