var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllersWithViews();
services.AddRazorPages();

/* ---------------------------------------------------------------------------- */

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/MVC/Error");
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapControllerRoute("default", "{controller}/{action}/{id?}");

app.Run();
