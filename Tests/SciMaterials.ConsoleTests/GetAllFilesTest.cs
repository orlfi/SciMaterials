using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.WebApi.Clients.Files;

namespace SciMaterials.ConsoleTests;

public class GetAllFilesTest
{
    private readonly IFilesClient _filesClient;

    public GetAllFilesTest(IFilesClient filesClient, IUnitOfWork<SciMaterialsContext> unitOfWork)
    {
        _filesClient = filesClient;
    }

    public async Task Get()
    {
        var result = await _filesClient.GetAllAsync();
        if (result.Succeeded)
            foreach (var item in result.Data)
                Console.WriteLine($"{item.Id} >>> {item.Name}");
        else
            Console.WriteLine(string.Join(";", result.Messages));
    }
}
