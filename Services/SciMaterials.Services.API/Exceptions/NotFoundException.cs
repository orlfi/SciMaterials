using SciMaterials.Contracts.Enums;

namespace SciMaterials.Services.API.Exceptions;

public class NotFoundException : ApiException
{
    public NotFoundException(string message) : base((int)ResultCodes.NotFound, message) { }
}
