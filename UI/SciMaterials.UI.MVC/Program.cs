using SciMaterials.Services.API.Configuration;
using SciMaterials.Services.API.Extensions;
using SciMaterials.UI.MVC.API.Middlewares;
using SciMaterials.UI.MVC.API.Extensions;
using SciMaterials.Services.Database.Extensions;
using SciMaterials.UI.MVC.Identity.Extensions;
using SciMaterials.WebApi.Clients.Identity.Extensions;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.Services.ShortLinks;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Logging.json", true);

var config = builder.Configuration;

builder.WebHost.ConfigureKestrel(opt =>
{
    var api_settings = config.GetSection(ApiSettings.SectionName);
    //var fileSize = apiSettings.GetValue<long>("MaxFileSize");
    opt.Limits.MaxRequestBodySize = api_settings.GetValue<long>("MaxFileSize");
});

var services = builder.Services;

services.AddControllersWithViews();
services.AddRazorPages();

services.ConfigureApplication(config);
services.AddDatabaseProviders(config);
services.AddApiServices(config);
services.AddSwagger();
services.AddAuthApiServices(config);
services.AddAuthDbInitializer();
services.AddAuthUtils();
services.AddIdentityClients(new Uri(config["WebAPI"]));

services.AddHttpContextAccessor();

services.AddAuthJwtAndSwaggerApiServices(builder.Configuration);

var app = builder.Build();

await app.InitializeDbAsync(config);

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/MVC/Error");
}

app.UseBlazorFrameworkFiles();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
        
app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapRazorPages();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.MapControllerRoute("default", "{controller}/{action=index}/{id?}");

app.MapPut("replace-link",
    async (string text, ILinkReplaceService linkReplaceService, LinkGenerator linkGenerator, IHttpContextAccessor context) =>
    {
        var result = await linkReplaceService.ShortenLinksAsync(text);
        return result;
    });

app.Run();

public partial class Program { }