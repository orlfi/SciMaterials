using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Auth;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.DAL.AUTH.InitializationDb;

public class AuthDbInitializer : IAuthDbInitializer
{
    private readonly ILogger<AuthDbContext> _logger;
    private readonly AuthDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthDbInitializer(
        ILogger<AuthDbContext> logger,
        AuthDbContext dbContext, 
        UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IConfiguration configuration)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }
    
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,"Initialize auth database start {Time}", DateTime.Now);
        await _dbContext.Database.MigrateAsync(cancellationToken);
        await AuthRolesInitializer.InitializeAsync(_userManager, _roleManager, _configuration);
        _logger.Log(LogLevel.Information,"Initialize auth database stop {Time}", DateTime.Now);
    }
}