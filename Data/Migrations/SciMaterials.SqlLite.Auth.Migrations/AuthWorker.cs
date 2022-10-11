using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.SqlLite.Auth.Migrations;

public class AuthWorker : IHostedService
{
    private readonly IDbContextFactory<AuthSqliteDbContext> _applicationContext;
    private CancellationTokenSource _cts;
    public AuthWorker(IDbContextFactory<AuthSqliteDbContext> applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        await using (var dbContext = await _applicationContext.CreateDbContextAsync(cancellationToken))
        {
            await dbContext.Database.MigrateAsync(_cts.Token);
        }

        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}