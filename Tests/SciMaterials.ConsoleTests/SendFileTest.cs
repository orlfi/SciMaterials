using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.UnitOfWork;

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
        var allCategories = await _unitOfWork.GetRepository<Category>().GetAllAsync();
        var categoryId = allCategories.First().Id;

        var allAuthors = await _unitOfWork.GetRepository<Author>().GetAllAsync();
        var authorId = allAuthors.First().Id;

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
            ContentTypeName = "text/plain",
            AuthorId = authorId
        };

        var result = await _filesClient.UploadAsync(fileStream, uploadFileRequest);
        Console.WriteLine(string.Join(";", result.Messages));
    }
}
