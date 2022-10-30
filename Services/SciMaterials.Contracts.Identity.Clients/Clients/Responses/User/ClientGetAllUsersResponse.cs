
namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

public class ClientGetAllUsersResponse : Result.Result
{
    public string? Message { get; set; }
    public List<string>? UserId { get; set; }
    public List<string>? UserNickNames { get; set; }
    public List<string>? UserEmails { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}