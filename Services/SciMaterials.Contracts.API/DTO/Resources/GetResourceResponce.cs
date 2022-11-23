using SciMaterials.Domain.Enums;

namespace SciMaterials.Contracts.API.DTO.Resources;

public record GetResourceResponse(Guid Id, string Name, string ShortInfo, ResourceType ResourceType);
