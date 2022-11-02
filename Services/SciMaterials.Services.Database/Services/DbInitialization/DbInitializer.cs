using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Services;

namespace SciMaterials.Services.Database.Services.DbInitialization;

public class DbInitializer : IDbInitializer
{
    private readonly SciMaterialsContext _db;
    private readonly ILogger<SciMaterialsContext> _Logger;

    public DbInitializer(SciMaterialsContext db, ILogger<SciMaterialsContext> logger)
    {
        _db = db;
        _Logger = logger;
    }

    public async Task<bool> DeleteDbAsync(CancellationToken Cancel = default)
    {
        _Logger.LogInformation("Deleting a database...");

        Cancel.ThrowIfCancellationRequested();

        try
        {
            var result = await _db.Database.EnsureDeletedAsync(Cancel).ConfigureAwait(false);
            return result;
        }
        catch (OperationCanceledException e)
        {
            _Logger.LogError(e, "Interrupting an operation when deleting a database");
            throw;
        }
        catch (Exception e)
        {
            _Logger.LogError(e, "Error during database initialization");
            throw;
        }
    }

    public async Task InitializeDbAsync(bool RemoveAtStart = false, bool UseDataSeeder = false, CancellationToken Cancel = default)
    {
        _Logger.LogInformation("Database initialization...");

        Cancel.ThrowIfCancellationRequested();

        try
        {
            if (RemoveAtStart) await DeleteDbAsync(Cancel).ConfigureAwait(false);

            var pending_migrations = await _db.Database.GetPendingMigrationsAsync(Cancel).ConfigureAwait(false);

            if (pending_migrations.Any()) 
                await _db.Database.MigrateAsync(Cancel).ConfigureAwait(false);

            if (UseDataSeeder)
                await InitializeDbAsync(Cancel).ConfigureAwait(false);
        }
        catch (OperationCanceledException e)
        {
            _Logger.LogError(e, "Interrupting an operation when deleting a database");
            throw;
        }
        catch (Exception e)
        {
            _Logger.LogError(e, "Error during database initialization");
            throw;
        }
    }

    private async Task InitializeDbAsync(CancellationToken Cancel = default)
    {
        await DataSeeder.SeedAsync(_db, Cancel).ConfigureAwait(false);
    }
}