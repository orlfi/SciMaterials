using Blazored.LocalStorage;

using Fluxor;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SciMaterials.UI.BWASM;
using SciMaterials.UI.BWASM.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services
    .AddAuthorizationCore()
    .AddScoped<IAuthenticationService, TestAuthenticationService>()
    .AddScoped<AuthenticationStateProvider, TestAuthenticationStateProvider>();

builder.Services
    .AddSingleton<FileUploadScheduleService>();

// State management
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools(reduxOptions => reduxOptions.Name = "SciMaterials");
});

await builder.Build().RunAsync();
