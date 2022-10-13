using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.ContentTypes;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.ContentTypes;

namespace SciMaterials.Services.API.Services.ContentTypes;

public class ContentTypeService : IContentTypeService
{
    private readonly ILogger<ContentTypeService> _logger;
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;

    public ContentTypeService(ILogger<ContentTypeService> logger, IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetContentTypeResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.GetRepository<ContentType>().GetAllAsync();
        var result = _mapper.Map<List<GetContentTypeResponse>>(categories);
        return result;
    }

    public async Task<Result<GetContentTypeResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<ContentType>().GetByIdAsync(id) is { } author)
            return _mapper.Map<GetContentTypeResponse>(author);

        return await Result<GetContentTypeResponse>.ErrorAsync((int)ResultCodes.NotFound, $"ContentType with ID {id} not found");
    }

    public async Task<Result<Guid>> AddAsync(AddContentTypeRequest request, CancellationToken cancellationToken = default)
    {
        var author = _mapper.Map<ContentType>(request);
        await _unitOfWork.GetRepository<ContentType>().AddAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(author.Id, "ContentType created");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> EditAsync(EditContentTypeRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<ContentType>().GetByIdAsync(request.Id) is not { })
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"ContentType with ID {request.Id} not found");

        var author = _mapper.Map<ContentType>(request);
        await _unitOfWork.GetRepository<ContentType>().UpdateAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(author.Id, "ContentType updated");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<ContentType>().GetByIdAsync(id) is not { } author)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"ContentType with ID {id} not found");

        await _unitOfWork.GetRepository<ContentType>().DeleteAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync($"ContentType with ID {author.Id} deleted");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }
}
