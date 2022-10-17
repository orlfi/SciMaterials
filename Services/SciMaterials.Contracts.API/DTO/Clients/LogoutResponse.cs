using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.DTO.Clients;

public class LogoutResponse : IResult
{
    public int Code { get; set; }
    public bool Succeeded { get; set; }
    public ICollection<string> Messages { get; set; }
}