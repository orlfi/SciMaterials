namespace SciMaterials.Contracts.Identity.API.Responses.User;

public class ClientRefreshTokenResponse : Result.Result
{
    public string? Message { get; set; }
    public string? RefreshToken { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}