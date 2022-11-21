using SciMaterials.Contracts.Identity.API.Responses.DTO;

namespace SciMaterials.Contracts.Identity.API.Responses.User;

public class ClientGetUserByEmailResponse : Result.Result
{
    public List<AuthUsers> Users { get; set; }
    public string? Message { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}