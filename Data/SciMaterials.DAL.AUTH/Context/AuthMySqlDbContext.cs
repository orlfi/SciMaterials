using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SciMaterials.DAL.AUTH.Context;

public class AuthMySqlDbContext : IdentityDbContext<IdentityUser>
{
    public AuthMySqlDbContext(DbContextOptions<AuthMySqlDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}