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
        Name = "1_C#10_Troelsen.pdf",
        ShortInfo = "Pro C# 10 with .NET 6: Foundational Principles and Practices in Programming 11st ed. Edition",
        Description = "Welcome to the most comprehensive foundational guide available on the topic of <a href=\"https://learn.microsoft.com/ru-ru/dotnet/csharp/\">C#</a> coding and <a href=\"https://dotnet.microsoft.com/en-us/\">.NET</a>. This book goes beyond 'do this, to achieve this' to drill down into the core stuff that makes a good developer, great. This expanded 11th edition delivers loads of new content on Entity Framework, Razor Pages, Web APIs and more.",
        Tags = "0853E794-8271-C3CA-5468-305D507E3D2A,7B46CD12-AC97-67F6-BC02-39F514C982C7,088E2112-1310-5662-B76C-5C912B98B047",
        Categories = "95B24213-7458-A856-12CC-523D2EB4C539,8556B663-3758-E42E-872A-2AA503745384",
    };
    //await updateFileTest.Update(editFileRequest);
    await updateFileTest.UpdateByContext(editFileRequest);
}

await using (var scope = host.Services.CreateAsyncScope())
{

    var updateFileTest = scope.ServiceProvider.GetService<UpdateFileTest>();
    // Удаляю 2 тега 7B46CD12-AC97-67F6-BC02-39F514C982C7,088E2112-1310-5662-B76C-5C912B98B047 и добавляю 47FD91E3-A91C-6578-678B-7A4EEB9DF9C9
    // категории оставляю
    var editFileRequest = new EditFileRequest()
    {
        Id = Guid.Parse("EAB719EF-BEEE-76C2-93CD-9163851E6C8A"),
        Name = "2_C#10_Troelsen.pdf",
        ShortInfo = "Pro C# 10 with .NET 6: Foundational Principles and Practices in Programming 11st ed. Edition",
        Description = "Welcome to the most comprehensive foundational guide available on the topic of <a href=\"https://learn.microsoft.com/ru-ru/dotnet/csharp/\">C#</a> coding and <a href=\"https://dotnet.microsoft.com/en-us/\">.NET</a>. This book goes beyond 'do this, to achieve this' to drill down into the core stuff that makes a good developer, great. This expanded 11th edition delivers loads of new content on Entity Framework, Razor Pages, Web APIs and more.",
        Tags = "0853E794-8271-C3CA-5468-305D507E3D2A,47FD91E3-A91C-6578-678B-7A4EEB9DF9C9",
        Categories = "95B24213-7458-A856-12CC-523D2EB4C539,8556B663-3758-E42E-872A-2AA503745384",
    };
    //await updateFileTest.Update(editFileRequest);
    await updateFileTest.UpdateByContext(editFileRequest);

}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
