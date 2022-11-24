using AutoMapper;

using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.DTO.Resources;
using SciMaterials.Contracts.API.Models;
using SciMaterials.Contracts.API.Settings;
using SciMaterials.DAL.Models.Base;

using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.Contracts.API.Mappings;

public class ResourceProfile : Profile
{
    public ResourceProfile()
    {
        CreateMap<Resource, GetResourceResponse>()
            .ReverseMap();
    }
}
