using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SciMaterials.AUTH.Utilits;
using SciMaterials.DAL.AUTH.Context;
using SciMaterials.DAL.AUTH.InitializationDb;
using SciMaterials.DAL.AUTH.Interfaces;

namespace SciMaterials.Auth.Registrations;

public static class AuthApiServices
{
    public static IServiceCollection AddAuthApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["AuthApiSettings:Provider"];
        
        services.AddDbContext<AuthDbContext>(opt => _ = provider switch
        {
            "Sqlite" => opt.UseSqlite(AuthConnectionStrings.Sqlite(configuration), optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly("SciMaterials.SqlLite.Auth.Migrations");
            }),
            "MySql" => opt.UseMySql(AuthConnectionStrings.MySql(configuration), new MySqlServerVersion(new Version(8,0,30)), 
            optionBuilder =>
            {
                optionBuilder.MigrationsAssembly("SciMaterials.MySql.Auth.Migrations");
            }),
            _ => throw new Exception($"Unsupported provider: {provider}")
        });

        services.AddIdentity<IdentityUser, IdentityRole>(opt =>
        {
            opt.Password.RequiredLength = 5;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireDigit = false;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();
        
        services.AddDatabaseDeveloperPageExceptionFilter();
        
        return services;
    }

    public static IServiceCollection AddDbInitializer(this IServiceCollection services)
    {
        return services.AddTransient<IAuthDbInitializer, AuthDbInitializer>();
    }
}