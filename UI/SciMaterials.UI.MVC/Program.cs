using Microsoft.EntityFrameworkCore;
using SciMaterials.Auth.Registrations;
using SciMaterials.DAL.AUTH.Interfaces;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.InitializationDb.Interfaces;
using SciMaterials.Services.API.Configuration;
using SciMaterials.Services.API.Extensions;
using SciMaterials.UI.MVC.API.Middlewares;
using SciMaterials.UI.MVC.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var apiSettings = builder.Configuration.GetSection(ApiSettings.SectionName);
    var fileSize = apiSettings.GetValue<long>("MaxFileSize");
    serverOptions.Limits.MaxRequestBodySize = apiSettings.GetValue<long>("MaxFileSize");
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSwagger(builder.Configuration);
builder.Services.ConfigureApplication(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddAuthApiServices(builder.Configuration);
builder.Services.AddDbInitializer();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    if (builder.Configuration["DbProvider"].Equals("SQLite"))
    {
        var context = scope.ServiceProvider.GetRequiredService<SciMaterialsContext>();
        await context.Database.MigrateAsync().ConfigureAwait(false);
    }
    
    var authDb = scope.ServiceProvider.GetRequiredService<IAuthDbInitializer>();
    await authDb.InitializeAsync(builder.Configuration);
    
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await dbInitializer.InitializeDbAsync(removeAtStart: true).ConfigureAwait(false);
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/MVC/Error");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapControllerRoute("default", "{controller}/{action=index}/{id?}");

app.Run();
