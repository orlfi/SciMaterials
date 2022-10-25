using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SciMaterials.Contracts.API.Services.Identity;
using SciMaterials.Contracts.Auth;
using SciMaterials.DAL.AUTH.Context;
using SciMaterials.DAL.AUTH.InitializationDb;
using SciMaterials.UI.MVC.Identity.Services;
using SciMaterials.WebApi.Clients.Identity;

namespace SciMaterials.UI.MVC.Identity.Extensions;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddAuthApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["AuthApiSettings:Provider"];
        
        services.AddDbContext<AuthDbContext>(opt => _ = provider switch
        {
            "Sqlite" => opt.UseSqlite(AuthConnectionStrings.Sqlite(configuration), OptionsBuilder =>
            {
                OptionsBuilder.MigrationsAssembly("SciMaterials.SqlLite.Auth.Migrations");
            }),
            "MySql" => opt.UseMySql(AuthConnectionStrings.MySql(configuration), new MySqlServerVersion(new Version(8,0,30)), 
            OptionBuilder =>
            {
                OptionBuilder.MigrationsAssembly("SciMaterials.MySql.Auth.Migrations");
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
        
        services.AddHttpContextAccessor();
        
        return services;
    }

    public static IServiceCollection AddAuthJwtAndSwaggerApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title   = "SciMaterials",
                Version = "v1.1",
            });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
                Name        = "Authorization",
                In          = ParameterLocation.Header,
                Type        = SecuritySchemeType.Http,
                Scheme      = "Bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        
        services.AddAuthentication(x => 
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken            = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                    configuration.GetSection("AuthApiSettings:SecretTokenKey:key").Value)),
                ValidateIssuer           = false,
                ValidateAudience         = false,
                ValidateLifetime         = false,
                ValidateIssuerSigningKey = true,
                ClockSkew                = TimeSpan.Zero
            };
        });

        services.AddAuthorization();
        
        return services;
    }
    
    public static IServiceCollection AddAuthDbInitializer(this IServiceCollection services)
    {
        return services.AddScoped<IAuthDbInitializer, AuthDbInitializer>();
    }
    
    public static IServiceCollection AddAuthUtils(this IServiceCollection services)
    {
        return services.AddSingleton<IAuthUtils, AuthUtils>();
    }

    public static IServiceCollection AddAuthHttpClient(this IServiceCollection services)
    {
        services
            .AddHttpClient()
            .AddSingleton<IIdentityClient, IdentityClient>();

        return services;
    }
}