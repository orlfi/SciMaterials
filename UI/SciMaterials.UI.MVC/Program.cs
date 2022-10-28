using SciMaterials.Services.API.Configuration;
using SciMaterials.Services.API.Extensions;
using SciMaterials.UI.MVC.API.Middlewares;
using SciMaterials.UI.MVC.API.Extensions;
using SciMaterials.Services.Database.Extensions;
using SciMaterials.AUTH.Extensions;
using SciMaterials.Contracts.API.Services.Identity;
using SciMaterials.WebApi.Clients.Identity;
using SciMaterials.WebAPI.LinkSearch.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(opt =>
{
    var apiSettings = builder.Configuration.GetSection(ApiSettings.SectionName);
    //var fileSize = apiSettings.GetValue<long>("MaxFileSize");
    opt.Limits.MaxRequestBodySize = apiSettings.GetValue<long>("MaxFileSize");
});

// Add services to the container.
var services = builder.Services;
services.AddControllersWithViews();
services.AddRazorPages();
services.AddSwagger(builder.Configuration);
services.ConfigureApplication(builder.Configuration);
services.AddApiServices(builder.Configuration);
services.AddAuthApiServices(builder.Configuration);
services.AddApiLinkSearch(builder.Configuration);
services.AddAuthDbInitializer();
services.AddAuthUtils();
services.AddHttpClient();
services.AddSingleton<IIdentityClient, IdentityClient>();

var app = builder.Build();

await app.UseInitializationDbAsync(builder.Configuration);

if (app.Environment.IsDevelopment())
{
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
app.UseAuthorization();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapControllerRoute("default", "{controller}/{action=index}/{id?}");
app.Run();

// Required mark for integration tests
public partial class Program{}