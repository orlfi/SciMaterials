using Blazored.LocalStorage;

using Fluxor;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

using SciMaterials.Contracts.WebApi.Clients.Categories;
using SciMaterials.Contracts.WebApi.Clients.ContentTypes;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.Contracts.WebApi.Clients.Tags;
using SciMaterials.UI.BWASM;
using SciMaterials.UI.BWASM.Services;
using SciMaterials.WebApi.Clients.Categories;
using SciMaterials.WebApi.Clients.ContentTypes;
using SciMaterials.WebApi.Clients.Files;
using SciMaterials.WebApi.Clients.Tags;
using SciMaterials.WebApi.Clients.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Api
//builder.Services.AddApiClients(new Uri(builder.HostEnvironment.BaseAddress));
builder.Services
    .AddApiClient<IFilesClient, FilesClient>(BaseRoutes.FilesApi)
    .AddApiClient<ITagsClient, TagsClient>(BaseRoutes.TagsApi)
    .AddApiClient<ICategoriesClient, CategoriesClient>(BaseRoutes.CategoriesApi)
    .AddApiClient<IContentTypesClient, ContentTypesClient>(BaseRoutes.ContentTypesApi);

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
    options.WithLifetime(StoreLifetime.Singleton);
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools(reduxOptions => reduxOptions.Name = "SciMaterials");
});

await builder.Build().RunAsync();

static class BaseRoutes
{
    private const string MvcRoute = "http://localhost:5185";

    public const string FilesApi = MvcRoute;
    public const string TagsApi = MvcRoute;
    public const string CategoriesApi = MvcRoute;
    public const string ContentTypesApi = MvcRoute;
}