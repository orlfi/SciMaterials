using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Resources;
using SciMaterials.Contracts;
using SciMaterials.Contracts.API.Services.Tags;
using SciMaterials.DAL.Models.Base;

namespace SciMaterials.Services.API.Services.Resources;

public class ResourcesService : ApiServiceBase, IResourceService
{
    public ResourcesService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<ResourcesService> logger)
        : base(unitOfWork, mapper, logger) { }

    public async Task<Result<IEnumerable<GetResourceResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var resources = await _unitOfWork.GetRepository<Resource>().GetAllAsync();
        var result = _mapper.Map<List<GetResourceResponse>>(resources);
        return result;
    }

    public async Task<PageResult<GetResourceResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var resources = await _unitOfWork.GetRepository<Resource>().GetPageAsync(pageNumber, pageSize);
        var totalCount = await _unitOfWork.GetRepository<Resource>().GetCountAsync();
        var result = _mapper.Map<List<GetResourceResponse>>(resources);
        return (result, totalCount);
    }

    public async Task<Result<GetResourceResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Resource>().GetByIdAsync(id) is not { } resource)
        {
            return LoggedError<GetResourceResponse>(
                Errors.Api.Resource.NotFound,
                "Resource with ID {id} not found",
                id);
        }

        var result = _mapper.Map<GetResourceResponse>(resource);
        return result;
    }
}
