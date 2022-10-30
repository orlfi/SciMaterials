
namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

public class ClientEditUserNameByEmailResponse : Result.Result
{
    public string? NewToken { get; set; }
    public string? Message { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}