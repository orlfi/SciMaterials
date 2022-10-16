#region usings

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SciMaterials.ConsoleTests;
using SciMaterials.ConsoleTests.Extensions;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.Contracts.Result;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Services.API.Extensions;
using SciMaterials.Services.Database.Services.DbInitialization;
using SciMaterials.WebApi.Clients.Files;
using File = SciMaterials.DAL.Models.File;
#endregion

const string path = @"d:\tmp\test.txt";
const string baseAddress = "http://localhost:5185/";

static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
    .ConfigureServices(ConfigureServices)
    .ConfigureAppConfiguration(app => app.AddJsonFile("appsettings.json")
        .AddUserSecrets<Program>());

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddContextMultipleProviders(context);
    services.AddTransient<IDbInitializer, DbInitializer>();
    services.AddScoped<AddFileWithCategories>();
    services.AddApiServices(context.Configuration);
    services.AddScoped<SendFileTest>();
    services.AddHttpClient<IFilesClient, FilesClient>("FilesClient",
        client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });
}
using IHost host = CreateHostBuilder(args).Build();

await using (var scope = host.Services.CreateAsyncScope())
{
    // await dbInitializer.InitializeDbAsync(removeAtStart: false, useDataSeeder: false);
    // var service = scope.ServiceProvider.GetRequiredService<AddFileWithCategories>();
    // await service.AddFileToDatabase("test.txt");

    var sendFileTest = scope.ServiceProvider.GetRequiredService<SendFileTest>();
    await sendFileTest.SendFileAsync("test.txt");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
