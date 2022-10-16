using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.WebApi.Clients.Files;

namespace SciMaterials.ConsoleTests;

public class SendFileTest
{
    private readonly IFilesClient _filesClient;
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;

    public SendFileTest(IFilesClient filesClient, IUnitOfWork<SciMaterialsContext> unitOfWork)
    {
        _filesClient = filesClient;
        _unitOfWork = unitOfWork;
    }

    public async Task SendFileAsync(string path)
    {
        Guid categoryId;
        categoryId = (await _unitOfWork.GetRepository<Category>().GetAllAsync()).First().Id;

        var fileInfo = new FileInfo(path);
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
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

        var result = await _filesClient.UploadAsync(fileStream, uploadFileRequest);
        Console.WriteLine(string.Join(";", result.Messages));
    }
}
