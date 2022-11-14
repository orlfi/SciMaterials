using System.Reflection;

using AutoMapper;

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
    private readonly ILogger<UpdateFileTest> _logger;

    public UpdateFileTest(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<UpdateFileTest> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Update(EditFileRequest editFileRequest)
    {
        var existed = await _unitOfWork.GetRepository<File>().GetByIdAsync(editFileRequest.Id);
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
