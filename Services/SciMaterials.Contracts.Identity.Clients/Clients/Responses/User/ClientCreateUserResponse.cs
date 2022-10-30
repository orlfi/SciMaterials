
namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

public class ClientCreateUserResponse : Result.Result
{
    public string? ConfirmEmail { get; set; }
    public string? Message { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}