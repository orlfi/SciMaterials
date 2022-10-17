using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.WebApi.Clients.Files;

namespace SciMaterials.ConsoleTests;

public class EditFilesTest
{
    private readonly IFilesClient _filesClient;

    public EditFilesTest(IFilesClient filesClient, IUnitOfWork<SciMaterialsContext> unitOfWork)
    {
        _filesClient = filesClient;
    }

    public async Task Edit(EditFileRequest request)
    {
        var result = await _filesClient.EditAsync(request);
        if (result.Succeeded)
            Console.WriteLine($"Updated success >>> {result.Data.ToString()}");
        else
            Console.WriteLine(string.Join(";", result.Messages));
    }
}
