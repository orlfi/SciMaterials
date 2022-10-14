using SciMaterials.Contracts.API.DTO.Tags;

namespace SciMaterials.Contracts.API.Services.Tags;

public interface ITagService : IService<Guid, GetTagResponse>, IModifyService<AddTagRequest, EditTagRequest, Guid>
{
}