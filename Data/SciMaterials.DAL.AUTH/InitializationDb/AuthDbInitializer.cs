using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.AUTH.Context;
using SciMaterials.DAL.AUTH.Interfaces;

namespace SciMaterials.DAL.AUTH.InitializationDb;

public class AuthDbInitializer : IAuthDbInitializer
{
    private readonly ILogger<AuthDbContext> _logger;
    private readonly AuthDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthDbInitializer(
        ILogger<AuthDbContext> logger,
        AuthDbContext dbContext, 
        UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task InitializeAsync(
        IConfiguration configuration, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initialize auth database start");
        await _dbContext.Database.MigrateAsync(cancellationToken);
        await AuthRolesInitializer.InitializeAsync(_userManager, _roleManager, configuration);
        _logger.LogInformation("Initialize auth database stop");
    }
}