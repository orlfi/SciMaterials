#region usings
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SciMaterials.ConsoleTests;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.InitializationDb.Implementation;
using SciMaterials.DAL.InitializationDb.Interfaces;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Services.API.Extensions;
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
    services.AddApiServices(context.Configuration);
}

using IHost host = CreateHostBuilder(args).Build();

await using (var scope = host.Services.CreateAsyncScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    // await dbInitializer.InitializeDbAsync(removeAtStart: true);

    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<SciMaterialsContext>>();
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

    var file = (await unitOfWork.GetRepository<File>().GetAllAsync()).First();
    var request = mapper.Map<AddEditFileRequest>(file);
    
    if ((await SendFile(path, request)) is { Succeeded: true } result)
    {
        Console.WriteLine($"File {path} upladed with id {result.Data}");
    }
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();


static async Task<Result<Guid>> SendFile(string path, AddEditFileRequest uploadRequest)
{
    using HttpClient httpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5185/")
    };

    var fileClient = new FilesClient(httpClient);

    var payload = System.Text.Json.JsonSerializer.Serialize(uploadRequest);
    var result = await fileClient.UploadAsync(path, payload);
    return result;
}