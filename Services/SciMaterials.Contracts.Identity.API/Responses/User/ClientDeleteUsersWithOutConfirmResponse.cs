namespace SciMaterials.Contracts.Identity.API.Responses.User;

public class ClientDeleteUsersWithOutConfirmResponse : Result.Result
{
    public string? Message { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}