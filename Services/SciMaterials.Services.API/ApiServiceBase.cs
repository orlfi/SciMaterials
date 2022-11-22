using AutoMapper;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.UnitOfWork;


namespace SciMaterials.Services.API;

public abstract class ApiServiceBase: ServiceBase
{
    protected readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    protected readonly IMapper _mapper;

    protected ApiServiceBase(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger logger) 
        : base(logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
}