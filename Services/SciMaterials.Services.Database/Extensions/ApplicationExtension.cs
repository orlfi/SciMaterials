using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.Auth;
using SciMaterials.Contracts.Database.Configuration;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.DAL.Contexts;

namespace SciMaterials.Services.Database.Extensions;

public static class ApplicationExtension
{
    public static async Task<IApplicationBuilder> InitializeDbAsync(this IApplicationBuilder app, IConfiguration configuration)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var db_setting = configuration.GetSection("DbSettings").Get<DbSettings>();
        
        var db_domain = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await db_domain.InitializeDbAsync(db_setting.RemoveAtStart, db_setting.UseDataSeeder);

        var auth_db = scope.ServiceProvider.GetRequiredService<IAuthDbInitializer>();
        await auth_db.InitializeAsync();
        
        return app;
    }
}