using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using Category = SciMaterials.DAL.Models.Category;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Categories;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Services.API.Services.Categories;

public class CategoryService : ICategoryService
{
    private readonly ILogger<CategoryService> _logger;
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(ILogger<CategoryService> logger, IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetCategoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync();
        var result = _mapper.Map<List<GetCategoryResponse>>(categories);
        return result;
    }

    public async Task<Result<GetCategoryResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);

        if (category is null)
            return await Result<GetCategoryResponse>.ErrorAsync((int)ResultCodes.NotFound, $"Category with ID {id} not found");

        var result = _mapper.Map<GetCategoryResponse>(category);
        return result;
    }

    public async Task<Result<Guid>> AddEditAsync(AddEditCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id is null || request.Id == Guid.Empty)
        {
            var category = _mapper.Map<Category>(request);
            category.CreatedAt = DateTime.Now;
            return await AddAsync(category);
        }

        return await UpdateAsync(request.Id.Value);
    }
    private async Task<Result<Guid>> AddAsync(Category category)
    {
        await _unitOfWork.GetRepository<Category>().AddAsync(category);

        if (await _unitOfWork.SaveContextAsync() <= 0)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");

        return await Result<Guid>.SuccessAsync(category.Id, "Category created");
    }

    private async Task<Result<Guid>> UpdateAsync(Guid id)
    {
        var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);

        if (category is null)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Category with ID {id} not found");

        await _unitOfWork.GetRepository<Category>().UpdateAsync(category);

        if (await _unitOfWork.SaveContextAsync() <= 0)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");

        return await Result<Guid>.SuccessAsync(category.Id, "Category updated");
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<Category>().GetByIdAsync(id) is Category category)
        {
            await _unitOfWork.GetRepository<Category>().DeleteAsync(category);

            if (await _unitOfWork.SaveContextAsync() <= 0)
                return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");

            return await Result<Guid>.SuccessAsync($"Category with ID {category.Id} deleted");
        }

        return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Category with ID {id} not found");
    }
}
