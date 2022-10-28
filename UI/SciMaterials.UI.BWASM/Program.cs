using Blazored.LocalStorage;

using Fluxor;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;

using SciMaterials.Contracts.WebApi.Clients.Authors;
using SciMaterials.Contracts.WebApi.Clients.Categories;
using SciMaterials.Contracts.WebApi.Clients.ContentTypes;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.Contracts.WebApi.Clients.Tags;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.Identity.Clients.Clients;
using SciMaterials.UI.BWASM;
using SciMaterials.UI.BWASM.Extensions;
using SciMaterials.UI.BWASM.Models;
using SciMaterials.UI.BWASM.Models.Validations;
using SciMaterials.UI.BWASM.Services;
using SciMaterials.WebApi.Clients.Authors;
using SciMaterials.WebApi.Clients.Categories;
using SciMaterials.WebApi.Clients.ContentTypes;
using SciMaterials.WebApi.Clients.Files;
using SciMaterials.WebApi.Clients.Tags;
using SciMaterials.UI.BWASM.Services.Identity;
using SciMaterials.UI.BWASM.Services.PoliciesAuthentication;
using SciMaterials.WebApi.Clients.Extensions;
using SciMaterials.WebApi.Clients.Identity;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Api
BaseRoutes.MvcRoute = builder.HostEnvironment.BaseAddress;

builder.Services
    .AddApiClient<IFilesClient, FilesClient>(BaseRoutes.FilesApi)
    .AddApiClient<ITagsClient, TagsClient>(BaseRoutes.TagsApi)
    .AddApiClient<ICategoriesClient, CategoriesClient>(BaseRoutes.CategoriesApi)
    .AddApiClient<IContentTypesClient, ContentTypesClient>(BaseRoutes.ContentTypesApi)
    .AddApiClient<IAuthorsClient, AuthorsClient>(BaseRoutes.AuthorsApi);

// Nuget
builder.Services
    .AddMudServices()
    .AddBlazoredLocalStorage();

// Api
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services
    .AddApiClients(new Uri(builder.HostEnvironment.BaseAddress))
    .AddScoped<JwtAuthenticationHandler>();
string identityRoot = builder.HostEnvironment.BaseAddress;
builder.Services.AddHttpClient<IIdentityClient, IdentityClient>(c => c.BaseAddress = new Uri(identityRoot))
    .AddHttpMessageHandler<JwtAuthenticationHandler>();
builder.Services.AddHttpClient<IRolesClient, IdentityClient>(c => c.BaseAddress = new Uri(identityRoot))
    .AddHttpMessageHandler<JwtAuthenticationHandler>();

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

// State management
builder.Services.AddFluxor(options =>
{
    options.WithLifetime(StoreLifetime.Singleton);
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools(reduxOptions => reduxOptions.Name = "SciMaterials");
});

await builder.Build().RunAsync();

static class BaseRoutes
{
    public static string MvcRoute { get; set; } = "http://localhost:5185";

    public static string FilesApi => MvcRoute;
    public static string TagsApi => MvcRoute;
    public static string CategoriesApi => MvcRoute;
    public static string ContentTypesApi => MvcRoute;
    public static string AuthorsApi => MvcRoute;
}