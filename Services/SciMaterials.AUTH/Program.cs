using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SciMaterials.Auth.Registrations;
using SciMaterials.DAL.AUTH.Context;
using SciMaterials.DAL.AUTH.InitializationDb;


namespace SciMaterials.Auth;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Сервис аутентификации SciMaterials",
                Version = "v1.1",
            });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }, 
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.RegisterAuthUtils();

        //!Временное решение!
        var useDataBase = builder.Configuration.GetSection("AuthDataBase:Use").Value;
        if (useDataBase.Equals("MySql"))
        {
            builder.Services.RegisterMySqlProvider(builder.Configuration);
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    //По умолчанию поставил так
                    options.Password.RequiredLength = 5; //минимальная длинна пароля
                    options.Password.RequireNonAlphanumeric = false; //требуется ли применять символы
                    options.Password.RequireLowercase = false; //требуются ли символы в нижнем регистре
                    options.Password.RequireUppercase = false; //требуются ли символя в верхнем регистре
                    options.Password.RequireDigit = false; //требуются ли применять цифры в пароле
                })
                .AddEntityFrameworkStores<AuthMySqlDbContext>()
                .AddDefaultTokenProviders();
        }
        if (useDataBase.Equals("Sqlite"))
        {
            builder.Services.RegisterSqliteProvider(builder.Configuration);
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    //По умолчанию поставил так
                    options.Password.RequiredLength = 5; //минимальная длинна пароля
                    options.Password.RequireNonAlphanumeric = false; //требуется ли применять символы
                    options.Password.RequireLowercase = false; //требуются ли символы в нижнем регистре
                    options.Password.RequireUppercase = false; //требуются ли символя в верхнем регистре
                    options.Password.RequireDigit = false; //требуются ли применять цифры в пароле
                })
                .AddEntityFrameworkStores<AuthSqliteDbContext>()
                .AddDefaultTokenProviders();
        }
        

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        
        builder.Services.AddHttpContextAccessor();
        
        builder.Services.AddAuthentication(x => 
            {
                x.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                        builder.Configuration.GetSection("SecretTokenKey:Key").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();
        
        var app = builder.Build();

        var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        
        using (var scope = scopeFactory.CreateScope())
        {
            //var auth_db      = scope.ServiceProvider.GetRequiredService<AuthMySqlDbContext>();
            //await auth_db.Database.MigrateAsync();

            var userManager  = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var rolesManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await AuthRolesInitializer.InitializeAsync(userManager, rolesManager, builder.Configuration);

        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        
        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}