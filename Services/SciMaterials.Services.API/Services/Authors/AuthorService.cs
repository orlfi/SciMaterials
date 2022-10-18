using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Authors;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Authors;

namespace SciMaterials.Services.API.Services.Authors;

public class AuthorService : IAuthorService
{
    private readonly ILogger<AuthorService> _logger;
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;

    public AuthorService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<AuthorService> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetAuthorResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.GetRepository<Author>().GetAllAsync();
        var result = _mapper.Map<List<GetAuthorResponse>>(categories);
        return result;
    }

    public Task<PageResult<GetAuthorResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<GetAuthorResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<Author>().GetByIdAsync(id) is { } author)
            return _mapper.Map<GetAuthorResponse>(author);

        return await Result<GetAuthorResponse>.ErrorAsync((int)ResultCodes.NotFound, $"Author with ID {id} not found");
    }

    public async Task<Result<Guid>> AddAsync(AddAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var author = _mapper.Map<Author>(request);
        await _unitOfWork.GetRepository<Author>().AddAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(author.Id, "Author created");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> EditAsync(EditAuthorRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<Author>().GetByIdAsync(request.Id) is not { })
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Author with ID {request.Id} not found");

        var author = _mapper.Map<Author>(request);
        await _unitOfWork.GetRepository<Author>().UpdateAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(author.Id, "Author updated");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<Author>().GetByIdAsync(id) is not { } author)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Author with ID {id} not found");

        await _unitOfWork.GetRepository<Author>().DeleteAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync($"Author with ID {author.Id} deleted");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }
}
