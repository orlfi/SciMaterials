using SciMaterials.Contracts.API.DTO.Authors;

namespace SciMaterials.Contracts.API.Services.Authors;

public interface IAuthorService : IService<Guid, GetAuthorResponse>, IModifyService<AddAuthorRequest, EditAuthorRequest, Guid>
{
}