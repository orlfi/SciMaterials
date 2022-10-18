using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Auth;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.DAL.AUTH.InitializationDb;

public class AuthDbInitializer : IAuthDbInitializer
{
    private readonly ILogger<AuthDbContext> _Logger;
    private readonly AuthDbContext _DBContext;
    private readonly UserManager<IdentityUser> _UserManager;
    private readonly RoleManager<IdentityRole> _RoleManager;
    private readonly IConfiguration _Configuration;

    public AuthDbInitializer(
        ILogger<AuthDbContext> Logger,
        AuthDbContext DBContext, 
        UserManager<IdentityUser> UserManager, 
        RoleManager<IdentityRole> RoleManager, 
        IConfiguration Configuration)
    {
        _Logger = Logger;
        _DBContext = DBContext;
        _UserManager = UserManager;
        _RoleManager = RoleManager;
        _Configuration = Configuration;
    }
    
    public async Task InitializeAsync(CancellationToken CancellationToken = default)
    {
        _Logger.Log(LogLevel.Information,"Initialize auth database start {Time}", DateTime.Now);

        await _DBContext.Database.MigrateAsync(CancellationToken);

        await AuthRolesInitializer.InitializeAsync(_UserManager, _RoleManager, _Configuration);

        _Logger.Log(LogLevel.Information,"Initialize auth database stop {Time}", DateTime.Now);
    }
}