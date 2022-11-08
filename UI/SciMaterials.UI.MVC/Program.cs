using SciMaterials.Services.API.Configuration;
using SciMaterials.Services.API.Extensions;
using SciMaterials.UI.MVC.API.Middlewares;
using SciMaterials.UI.MVC.API.Extensions;
using SciMaterials.Services.Database.Extensions;
using SciMaterials.UI.MVC.Identity.Extensions;
using SciMaterials.WebApi.Clients.Identity.Extensions;
using System.Configuration;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.Services.ShortLinks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Routing;
using System.Linq.Expressions;

var builder = WebApplication.CreateBuilder(args);

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

services.AddAuthApiServices(config);
services.AddAuthDbInitializer();
services.AddAuthUtils();
services.AddIdentityClients(new Uri(config["WebAPI"]));
services.AddAuthHttpClient();

services.AddScoped<ILinkReplaceService, LinkReplaceService>();
services.AddScoped<ILinkShortCutService, LinkShortCutService>();

services.AddHttpContextAccessor();

services.AddAuthJwtAndSwaggerApiServices(builder.Configuration);

builder.Services.AddCors(o => o.AddPolicy("test", p => p.WithOrigins("http://localhost:5159").AllowAnyMethod().AllowAnyHeader()));

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

app.UseCors("test");

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
        var httpContext = context.HttpContext;
        //var address = linkGenerator.GetUriByAction(httpContext, "GetById", "Links", new { hash = "fwef" }, "http");

        var result = await linkReplaceService.ShortenLinksAsync(text);

        return result;
    });

app.Run();

public partial class Program { }