using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.InitializationDb.Interfaces;

namespace SciMaterials.DAL.Extensions
{
    public static class ApplicationExtension
    {
        public static async Task<IApplicationBuilder> UseInitializationDbAsync(this IApplicationBuilder app, IConfiguration configuration)
        {
            await using var scope = app.ApplicationServices.CreateAsyncScope();

            if (configuration["DbProvider"].Equals("SQLite"))
            {
                var context = scope.ServiceProvider.GetRequiredService<SciMaterialsContext>();
                await context.Database.MigrateAsync().ConfigureAwait(false);
            }

            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await dbInitializer.InitializeDbAsync(removeAtStart: true).ConfigureAwait(false);

            return app;
        }
    }
}
