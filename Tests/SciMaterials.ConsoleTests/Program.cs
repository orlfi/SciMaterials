#region usings
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SciMaterials.ConsoleTests;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Services.API.Extensions;
using SciMaterials.Services.Database.Services;
using SciMaterials.Services.Database.Services.DbInitialization;
using File = SciMaterials.DAL.Models.File;
#endregion

const string path = @"d:\tmp\test.txt";

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
}

using IHost host = CreateHostBuilder(args).Build();

await using (var scope = host.Services.CreateAsyncScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await dbInitializer.InitializeDbAsync(removeAtStart: false, useDataSeeder: false);
    var service = scope.ServiceProvider.GetRequiredService<AddFileWithCategories>();
    await service.AddFileToDatabase("test.txt");

}
var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5185/") };
// var sendFileTest = new SendFileTest(httpClient, host.Services);
// await sendFileTest.SendFile("test.txt");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
