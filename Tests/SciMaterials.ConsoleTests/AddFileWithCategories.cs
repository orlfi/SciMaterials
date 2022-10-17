using Microsoft.EntityFrameworkCore;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;

namespace SciMaterials.ConsoleTests;

public class AddFileWithCategories
{
    private readonly IServiceProvider _services;
    private readonly SciMaterialsContext _context;

    public AddFileWithCategories(SciMaterialsContext context)
    {
        // _services = services;
        _context = context;
    }

    public async Task AddFileToDatabase(string path)
    {
        Guid categoryId = Guid.NewGuid();
        try
        {
            var author = await _context.Set<Author>().FirstAsync();
            var contentType = await _context.Set<ContentType>().FirstAsync();
            // var category = await _context.Set<Category>().FirstAsync();
            var category = await _context.Set<Category>()
                .Where(c => c.Id == new Guid("a8edbade-efe7-2a15-30a7-16c737c71190"))
                .AsNoTracking()
                .SingleAsync();


            var fileInfo = new FileInfo(path);
            var file = new DAL.Models.File
            {
                Id          = Guid.NewGuid(),
                Name        = fileInfo.Name,
                Title       = "Файл " + fileInfo.Name,
                Description = "Содержит файл " + fileInfo.Name,
                Size        = fileInfo.Length,
                Tags        = null,
                Categories  = new List<Category> { category },
                ContentType = contentType,
                Author      = author
            };

            await _context.Set<DAL.Models.File>().AddAsync(file);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + ">>>" + ex.InnerException.Message);
        }
    }
}
