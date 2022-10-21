using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SciMaterials.AUTH.Extensions;
using SciMaterials.Contracts.Auth;
using SciMaterials.DAL.AUTH.InitializationDb;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IAuthDbInitializer, AuthDbInitializer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
        
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title   = "Сервис аутентификации SciMaterials",
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
        
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5185")
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});

builder.Services.AddAuthApiServices(builder.Configuration);
        
builder.Services.AddAuthUtils();
        
builder.Services.AddHttpContextAccessor();
        
builder.Services.AddAuthentication(x => 
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
                builder.Configuration.GetSection("AuthApiSettings:SecretTokenKey:key").Value)),
            ValidateIssuer           = false,
            ValidateAudience         = false,
            ValidateLifetime         = false,
            ValidateIssuerSigningKey = true,
            ClockSkew                = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
        
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var authDb = scope.ServiceProvider.GetRequiredService<IAuthDbInitializer>();
    await authDb.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
        
app.UseAuthorization();

app.UseCors();

app.MapControllers();

await app.RunAsync();