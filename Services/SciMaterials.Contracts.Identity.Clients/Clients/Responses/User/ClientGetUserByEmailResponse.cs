
namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

public class ClientGetUserByEmailResponse : Result.Result
{
    public List<string>? UserId { get; set; }
    public List<string>? UserNickName { get; set; }
    public List<string>? UserEmail { get; set; }
    public string? Message { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}