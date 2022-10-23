using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Auth;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.DAL.AUTH.InitializationDb;

public class AuthDbInitializer : IAuthDbInitializer
{
    private readonly AuthDbContext _DBContext;
    private readonly UserManager<IdentityUser> _UserManager;
    private readonly RoleManager<IdentityRole> _RoleManager;
    private readonly IConfiguration _Configuration;
    private readonly ILogger<AuthDbContext> _Logger;

    public AuthDbInitializer(
        AuthDbContext DBContext, 
        UserManager<IdentityUser> UserManager, 
        RoleManager<IdentityRole> RoleManager, 
        IConfiguration Configuration,
        ILogger<AuthDbContext> Logger)
    {
        _DBContext = DBContext;
        _UserManager = UserManager;
        _RoleManager = RoleManager;
        _Configuration = Configuration;
        _Logger = Logger;
    }
    
    public async Task InitializeAsync(bool RemoveAtStart = false, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information,"Initialize auth database start {Time}", DateTime.Now);

        if (RemoveAtStart)
            await _DBContext.Database.EnsureDeletedAsync(Cancel).ConfigureAwait(false);

        var pending_migrations = await _DBContext.Database.GetPendingMigrationsAsync(Cancel).ConfigureAwait(false);
        if(pending_migrations.Any())
            await _DBContext.Database.MigrateAsync(Cancel);

        await AuthRolesInitializer.InitializeAsync(_UserManager, _RoleManager, _Configuration);

        _Logger.Log(LogLevel.Information,"Initialize auth database stop {Time}", DateTime.Now);
    }
}