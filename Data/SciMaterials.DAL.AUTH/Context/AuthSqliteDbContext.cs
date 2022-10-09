using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SciMaterials.DAL.AUTH.Context;

public class AuthSqliteDbContext : IdentityDbContext<IdentityUser>
{
    public AuthSqliteDbContext(DbContextOptions<AuthSqliteDbContext> options) : base(options)
    { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}