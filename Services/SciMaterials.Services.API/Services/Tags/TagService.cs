using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Tags;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Tags;

namespace SciMaterials.Services.API.Services.Tags;

public class TagService : ITagService
{
    private readonly ILogger<TagService> _logger;
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;

    public TagService(ILogger<TagService> logger, IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetTagResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.GetRepository<Tag>().GetAllAsync();
        var result = _mapper.Map<List<GetTagResponse>>(categories);
        return result;
    }

    public async Task<Result<GetTagResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<Tag>().GetByIdAsync(id) is { } author)
            return _mapper.Map<GetTagResponse>(author);

        return await Result<GetTagResponse>.ErrorAsync((int)ResultCodes.NotFound, $"Tag with ID {id} not found");
    }

    public async Task<Result<Guid>> AddAsync(AddTagRequest request, CancellationToken cancellationToken = default)
    {
        var author = _mapper.Map<Tag>(request);
        await _unitOfWork.GetRepository<Tag>().AddAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(author.Id, "Tag created");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> EditAsync(EditTagRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<Tag>().GetByIdAsync(request.Id) is not { })
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Tag with ID {request.Id} not found");

        var author = _mapper.Map<Tag>(request);
        await _unitOfWork.GetRepository<Tag>().UpdateAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(author.Id, "Tag updated");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<Tag>().GetByIdAsync(id) is not { } author)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Tag with ID {id} not found");

        await _unitOfWork.GetRepository<Tag>().DeleteAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync($"Tag with ID {author.Id} deleted");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }
}
