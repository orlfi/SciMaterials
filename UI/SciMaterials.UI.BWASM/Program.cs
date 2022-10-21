using Blazored.LocalStorage;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SciMaterials.UI.BWASM;
using SciMaterials.UI.BWASM.Models;
using SciMaterials.UI.BWASM.Models.Validations;
using SciMaterials.UI.BWASM.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Nuget
builder.Services
    .AddMudServices()
    .AddBlazoredLocalStorage();

// Api
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services
    .AddScoped<IAccountsService, TestAccountsService>()
    .AddScoped<IAuthoritiesService, TestAuthoritiesService>();

// Authentication
builder.Services
    .AddAuthorizationCore()
    .AddScoped<IAuthenticationService, TestAuthenticationService>()
    .AddScoped<AuthenticationStateProvider, TestAuthenticationStateProvider>()
    .AddSingleton<AuthenticationCache>()
    .AddSingleton<IAuthorizationHandler, AuthorityHandler>()
    .AddSingleton<IAuthorizationPolicyProvider, AuthorityPolicyProvider>();

// Validators // TODO: register with assembly scan
builder.Services
    .AddScoped<IValidator<SignUpForm>, SignUpFormValidator>()
    .AddScoped<IValidator<SignInForm>, SignInFormValidator>();

// Background
builder.Services
    .AddSingleton<FileUploadScheduleService>();

await builder.Build().RunAsync();
