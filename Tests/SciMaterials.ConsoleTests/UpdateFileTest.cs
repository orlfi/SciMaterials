using System.Linq;
using System.Reflection;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.UnitOfWork;

using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.ConsoleTests;

public class UpdateFileTest
{
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly SciMaterialsContext _db;
    private readonly ILogger<UpdateFileTest> _logger;

    public UpdateFileTest(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<UpdateFileTest> logger, SciMaterialsContext db)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _db = db;
    }

    public async Task UpdateByContext(EditFileRequest editFileRequest)
    {
        var existed = await _db.Files
            .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings).SingleOrDefaultAsync(f => f.Id == editFileRequest.Id);
        var file = _mapper.Map(editFileRequest, existed);

        var tagIdStrings = editFileRequest.Tags.Split(',').Select(t => Guid.Parse(t));
        var fileTagsDictionary = existed.Tags.ToDictionary(t => t.Id);
        foreach (var tagId in tagIdStrings)
        {
            if (fileTagsDictionary.ContainsKey(tagId))
            {
                fileTagsDictionary.Remove(tagId);
                continue;
            }

            var tag = await _db.Tags.FindAsync(tagId);
            file.Tags.Add(tag);
        }
        foreach (var tag in fileTagsDictionary)
        {
            file.Tags.Remove(tag.Value);
        }

        var categoryIdStrings = editFileRequest.Categories.Split(',').Select(t => Guid.Parse(t));
        var fileCategoryDictionary = existed.Categories.ToDictionary(t => t.Id);
        foreach (var categoryId in categoryIdStrings)
        {
            if (fileCategoryDictionary.ContainsKey(categoryId))
            {
                fileCategoryDictionary.Remove(categoryId);
                continue;
            }

            var category = await _db.Categories.FindAsync(categoryId);
            file.Categories.Add(category);
        }
        foreach (var category in fileCategoryDictionary)
        {
            file.Categories.Remove(category.Value);
        }

        await _db.SaveChangesAsync();
    }

    public async Task UpdateByContext2(EditFileRequest editFileRequest)
    {
        var existed = await _db.Files
            .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings).SingleOrDefaultAsync(f => f.Id == editFileRequest.Id);
        var file = _mapper.Map(editFileRequest, existed);

        var tagIdStrings = editFileRequest.Tags.Split(',').Select(t => Guid.Parse(t)).ToList();
        var tags = await _db.Tags.Where(t => tagIdStrings.Contains(t.Id)).ToListAsync();
        file.Tags = tags;

        var categoryIdStrings = editFileRequest.Categories.Split(',').Select(t => Guid.Parse(t)).ToList();
        var categories = await _db.Categories.Where(c => categoryIdStrings.Contains(c.Id)).ToListAsync();
        file.Categories = categories;

        await _db.SaveChangesAsync();
    }

    public async Task Update(EditFileRequest editFileRequest)
    {
        var existed = await _unitOfWork.GetRepository<File>().GetByIdAsync(editFileRequest.Id, false, true);
        var file = _mapper.Map(editFileRequest, existed);

        var tagIdStrings = editFileRequest.Tags.Split(',').Select(t => Guid.Parse(t));
        foreach (var tagId in tagIdStrings)
        {
            var tag = await _unitOfWork.GetRepository<Tag>().GetByIdAsync(tagId);
            file.Tags.Add(tag);
        }

        var categoryIdStrings = editFileRequest.Categories.Split(',').Select(t => Guid.Parse(t));
        foreach (var categoryId in categoryIdStrings)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(categoryId);
            file.Categories.Add(category);
        }

        await _unitOfWork.GetRepository<File>().UpdateAsync(file);
        await _unitOfWork.SaveContextAsync();
    }
}
