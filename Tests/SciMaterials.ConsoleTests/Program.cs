#region usings

using System.Reflection;
using System.Threading.Channels;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SciMaterials.ConsoleTests;
using SciMaterials.ConsoleTests.Extensions;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.Contracts.ShortLinks.Settings;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Services.API.Extensions;
using SciMaterials.Services.Database.Services.DbInitialization;
using SciMaterials.Services.ShortLinks;
using SciMaterials.WebApi.Clients.Extensions;
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
    services.Configure<LinkShortCutOptions>(context.Configuration.GetSection("LinkShortCutOptions"));
    services.AddContextMultipleProviders(context);
    services.AddTransient<IDbInitializer, DbInitializer>();
    services.AddScoped<AddFileWithCategories>();
    services.AddApiServices(context.Configuration);
    services.AddScoped<GetAllFilesTest>();
    services.AddScoped<GetFileByIdTest>();
    services.AddScoped<EditFilesTest>();
    services.AddScoped<SendFileTest>();
    services.AddScoped<DownloadFileByIdTest>();
    services.AddScoped<UpdateFileTest>();
    services.AddScoped<ILinkReplaceService, LinkReplaceService>();
    services.AddScoped<ILinkShortCutService, LinkShortCutService>();
    services.AddApiClients(new Uri(baseAddress));
    services.AddHttpContextAccessor();
    services.AddAutoMapper(Assembly.GetExecutingAssembly());

}
using IHost host = CreateHostBuilder(args).Build();

await using (var scope = host.Services.CreateAsyncScope())
{

    var updateFileTest = scope.ServiceProvider.GetService<UpdateFileTest>();
    var editFileRequest = new EditFileRequest()
    {
        Id = Guid.Parse("EAB719EF-BEEE-76C2-93CD-9163851E6C8A"),
        Name = "C#10_Troelsen.pdf",
        ShortInfo = "Pro C# 10 with .NET 6: Foundational Principles and Practices in Programming 11st ed. Edition",
        Description = "Welcome to the most comprehensive foundational guide available on the topic of <a href=\"https://learn.microsoft.com/ru-ru/dotnet/csharp/\">C#</a> coding and <a href=\"https://dotnet.microsoft.com/en-us/\">.NET</a>. This book goes beyond 'do this, to achieve this' to drill down into the core stuff that makes a good developer, great. This expanded 11th edition delivers loads of new content on Entity Framework, Razor Pages, Web APIs and more.",
        Tags = "0853E794-8271-C3CA-5468-305D507E3D2A,7B46CD12-AC97-67F6-BC02-39F514C982C7,088E2112-1310-5662-B76C-5C912B98B047",
        Categories = "95B24213-7458-A856-12CC-523D2EB4C539,8556B663-3758-E42E-872A-2AA503745384",
    };
    await updateFileTest.Update(editFileRequest);

    //var linkReplaceService = scope.ServiceProvider.GetService<ILinkReplaceService>();
    //var text = "The target endpoint might be prepared to accept the <code>application/json</code> content type for additional data. It needs <a href=\"https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-model-binding\" target=\"_blank\" rel=\"noreferrer\">custom model binders</a> that deserializes the JSON content to the target type. In this case, the <code>Data</code> property is decorated with the <code>ModelBinder</code> attribute that takes the type of a custom binder.";
    //Console.WriteLine(text);

    //var updatedText = await linkReplaceService.ShortenLinksAsync(text);

    //var db = scope.ServiceProvider.GetRequiredService<SciMaterialsContext>();
    //var links = await db.Links.ToListAsync();
    //var authors = await db.Authors.ToListAsync();

    // await dbInitializer.InitializeDbAsync(removeAtStart: false, useDataSeeder: false);
    // var service = scope.ServiceProvider.GetRequiredService<AddFileWithCategories>();
    // await service.AddFileToDatabase("test.txt");

    //Console.WriteLine("Get all files:");
    //var getAllFilesTest = scope.ServiceProvider.GetRequiredService<GetAllFilesTest>();
    //await getAllFilesTest.Get();

    //Console.WriteLine("Get file by Id:");
    //var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<SciMaterialsContext>>();
    //var allFiles = await unitOfWork.GetRepository<File>().GetAllAsync();
    //var fileId = allFiles.First().Id;
    //var getFileByIdTest = scope.ServiceProvider.GetRequiredService<GetFileByIdTest>();
    //await getFileByIdTest.Get(fileId);

    //var file = allFiles.First();
    //var editFileRequest = new EditFileRequest
    //{
    //    Id = file.Id,
    //    Categories = string.Join(",", file.Categories.Select(item => item.Id)) ?? "",
    //    Tags = string.Join(",", file.Tags.Select(item => item.Name)) ?? "",
    //    Description = "Тестовое описание",
    //    Title = "Тестовый заголовок"
    //};
    //var editFilesTest = scope.ServiceProvider.GetRequiredService<EditFilesTest>();
    //await editFilesTest.Edit(editFileRequest);
    //await getFileByIdTest.Get(file.Id);

    // var sendFileTest = scope.ServiceProvider.GetRequiredService<SendFileTest>();
    // await sendFileTest.SendFileAsync("test.txt");

    //var downloadFileByIdTest = scope.ServiceProvider.GetRequiredService<DownloadFileByIdTest>();
    //await downloadFileByIdTest.Download(Guid.Parse("29ED78FA-AE3E-4A5E-9981-F1816EE4E664"));
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
