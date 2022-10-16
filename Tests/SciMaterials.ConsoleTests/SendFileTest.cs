using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.UnitOfWork;

namespace SciMaterials.ConsoleTests;

public class SendFileTest
{
    private readonly FilesClient _filesClient;
    private readonly IServiceProvider _services;

    public SendFileTest(HttpClient httpClient, IServiceProvider services)
    {
        _filesClient = new FilesClient(httpClient);
        _services = services;
    }

    public async Task SendFile(string path)
    {
        Guid categoryId;
        await using (var scope = _services.CreateAsyncScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<SciMaterialsContext>>();
            categoryId = (await unitOfWork.GetRepository<Category>().GetAllAsync()).First().Id;
        }
        var fileInfo = new FileInfo(path);
        // var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name) + ".ee";
        var fileName = fileInfo.Name;
        var uploadFileRequest = new UploadFileRequest
        {
            Name = fileName,
            Title = "Файл " + fileInfo.Name,
            Description = "Содержит файл " + fileInfo.Name,
            Size = fileInfo.Length,
            Tags = "текст,книга,txt",
            Categories = categoryId.ToString(),
            ContentTypeName = "text/plain"
        };
        var metadataJson = JsonSerializer.Serialize(uploadFileRequest);

        var result = await _filesClient.UploadAsync(path, metadataJson);
        Console.WriteLine(string.Join(";", result.Messages));
    }
}
