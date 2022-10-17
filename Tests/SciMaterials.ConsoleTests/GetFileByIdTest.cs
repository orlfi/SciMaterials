using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.WebApi.Clients.Files;

namespace SciMaterials.ConsoleTests;

public class GetFileByIdTest
{
    private readonly IFilesClient _filesClient;
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;

    public GetFileByIdTest(IFilesClient filesClient, IUnitOfWork<SciMaterialsContext> unitOfWork)
    {
        _filesClient = filesClient;
        _unitOfWork = unitOfWork;
    }

    public async Task Get(Guid fileId)
    {
        var result = await _filesClient.GetByIdAsync(fileId);
        if (result.Succeeded)
            Console.WriteLine($"{result.Data.Id} >>> {result.Data.Name}");
        else
            Console.WriteLine(string.Join(";", result.Messages));
    }
}
