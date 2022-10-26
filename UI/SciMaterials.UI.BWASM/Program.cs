using Blazored.LocalStorage;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;

using SciMaterials.Contracts.API.DTO.AuthRoles;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.API.DTO.Clients;
using SciMaterials.Contracts.API.Services.Identity;
using SciMaterials.UI.BWASM;
using SciMaterials.UI.BWASM.Models;
using SciMaterials.UI.BWASM.Models.Validations;
using SciMaterials.UI.BWASM.Services;
using SciMaterials.UI.BWASM.Services.Identity;
using SciMaterials.UI.BWASM.Services.PoliciesAuthentication;
using SciMaterials.WebApi.Clients.Extensions;
using SciMaterials.WebApi.Clients.Identity;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Nuget
builder.Services
    .AddMudServices()
    .AddBlazoredLocalStorage();

// Api
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddApiClients(new Uri(builder.HostEnvironment.BaseAddress));
string identityRoot = builder.HostEnvironment.BaseAddress;
builder.Services.AddHttpClient<IIdentityUserClient<IdentityClientResponse, AuthUserRequest>, IdentityClient>(c => c.BaseAddress = new Uri(identityRoot));
builder.Services.AddHttpClient<IIdentityRolesClient<IdentityClientResponse, AuthRoleRequest>, IdentityClient>(c => c.BaseAddress = new Uri(identityRoot));

builder.Services
    .AddScoped<IRolesService, IdentityRolesService>()
    .AddScoped<IAccountsService, IdentityAccountsService>()
//    .AddScoped<IAuthoritiesService, TestAuthoritiesService>()
 ;

// Authentication
builder.Services
    .AddAuthorizationCore()
    .AddScoped<IAuthenticationService, IdentityAuthenticationService>()
    .AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>()
    //.AddSingleton<AuthenticationCache>()
    //.AddSingleton<IAuthorizationHandler, AuthorityHandler>()
    //.AddSingleton<IAuthorizationPolicyProvider, AuthorityPolicyProvider>()
    ;

// Validators // TODO: register with assembly scan
builder.Services
    .AddScoped<IValidator<SignUpForm>, SignUpFormValidator>()
    .AddScoped<IValidator<SignInForm>, SignInFormValidator>();

// Background
builder.Services
    .AddSingleton<FileUploadScheduleService>();

await builder.Build().RunAsync();
