using System.Configuration;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using SciMaterials.Contracts.Identity.API;
using SciMaterials.DAL.AUTH.Context;
using SciMaterials.DAL.AUTH.Contracts;
using SciMaterials.DAL.AUTH.InitializationDb;
using SciMaterials.DAL.Contracts.Configuration;
using SciMaterials.MySql.Auth.Migrations;
using SciMaterials.Postgres.Auth.Migrations;
using SciMaterials.SqlLite.Auth.Migrations;
using SciMaterials.UI.MVC.Identity.Services;
using SciMaterials.WebApi.Clients.Identity;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace SciMaterials.UI.MVC;

public static class IdentityRegister
{
    /// <summary>Метод расширения по установке сервисов БД Identity</summary>
    /// <param name="Services">Сервисы</param>
    /// <param name="Configuration">Конфигурации</param>
    /// <returns>Коллекция сервисов</returns>
    /// <exception cref="Exception"></exception>
    public static IServiceCollection AddIdentityDatabase(this IServiceCollection Services, IConfiguration Configuration)
    {
        var providerName = Configuration.GetSection("IdentityDatabase").GetValue<string>(nameof(DatabaseSettings.Provider));

        switch (providerName.ToLower())
        {
            case "postgresql":
                Services.AddIdentityPostgres();
                break;
            case "mysql":
                Services.AddIdentityMySql();
                break;
            case "sqlite":
                Services.AddIdentitySQLite();
                break;
            default:
                throw new Exception($"Unsupported provider: {providerName}");
        }

        Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
        {
            opt.Password.RequiredLength = 5;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireDigit = false;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        Services.AddScoped<IdentityDatabaseManager>();

        return Services;
    }

    /// <summary>Метод расширения по установке сервисов для JWT и Swagger</summary>
    /// <param name="Services">Сервисы</param>
    /// <param name="Configuration">Конфигурации</param>
    /// <returns>Коллекция сервисов</returns>
    public static void ConfigureIdentityInSwagger(this SwaggerGenOptions Options)
    {
        Options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });
        Options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    }

    /// <summary>
    /// Метод расширения по установке утилит для работы с Identity
    /// </summary>
    /// <param name="Services">Сервисы</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddIdentityServices(this IServiceCollection Services, IConfiguration Configuration)
    {
        Services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                        Configuration.GetValue<string>("IdentitySettings:SecretTokenKey"))),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        Services.AddAuthorization();

        return Services.AddSingleton<IAuthUtils, AuthUtils>();
    }

    public static IServiceCollection AddIdentityClients(this IServiceCollection services, string serverUrl)
    {
        services.AddHttpClient<IIdentityClient, IdentityClient>("IdentityClient", c =>
        {
            c.BaseAddress = new Uri(serverUrl);
        });
        return services;
    }

    /// <summary>
    /// Метод расширения по установке утилит для работы с Identity
    /// </summary>
    /// <param name="Services">Сервисы</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddAuthUtils(this IServiceCollection Services)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

    /// <summary>
    /// Статический класс по формированию строк подключения к БД
    /// </summary>
    public static class AuthConnectionStrings
    {
        /// <summary>
        /// Метод формирует строку подключения для БД MYSQL
        /// </summary>
        /// <param name="configuration">Конфигурация</param>
        /// <returns>Сформированную строку подключения</returns>
        public static string MySql(IConfiguration configuration)
        {
            var connection_string = configuration.GetConnectionString("AuthDbConnection");
            var db_config = configuration.GetSection("AuthApiSettings:MySqlDbConfig");
            var builder = new MySqlConnectionStringBuilder(connection_string);
            builder.Server = db_config["server"];
            builder.Database = db_config["database"];
            builder.Port = Convert.ToUInt32(db_config["port"]);
            builder.UserID = db_config["userid"];
            builder.Password = db_config["password"];
            connection_string = builder.ConnectionString;

        var db_setting = configuration.GetSection("IdentityDatabase").Get<DatabaseSettings>();

        /// <summary>
        /// Метод формирует строку подключения для БД SQLITE
        /// </summary>
        /// <param name="configuration">Конфигурация</param>
        /// <returns>Сформированную строку подключения</returns>
        public static string Sqlite(IConfiguration configuration)
        {
            var connection_string = configuration.GetConnectionString("AuthDbConnection");
            var db_config = configuration.GetSection("AuthApiSettings:SqliteDbConfig");
            var builder = new SqliteConnectionStringBuilder(connection_string);
            builder.DataSource = db_config["datasource"];
            connection_string = builder.ConnectionString;

        if (db_setting.RemoveAtStart) await manager.DeleteDatabaseAsync();

        /// <summary>
        /// Метод формирует строку подключения для БД POSTGRESSQL
        /// </summary>
        /// <param name="configuration">Конфигурация</param>
        /// <returns>Сформированную строку подключения</returns>
        public static string PostgresSql(IConfiguration configuration)
        {
            var connection_string = configuration.GetConnectionString("AuthDbConnection");
            var db_config = configuration.GetSection("AuthApiSettings:PostgresSqlDbConfig");
            var builder = new NpgsqlConnectionStringBuilder(connection_string);
            builder.Host = db_config["host"];
            builder.Database = db_config["database"];
            builder.Port = Convert.ToInt32(db_config["port"]);
            builder.Username = db_config["userid"];
            builder.Password = db_config["password"];
            connection_string = builder.ConnectionString;

        if (db_setting.UseDataSeeder) await manager.SeedDatabaseAsync();
    }
}