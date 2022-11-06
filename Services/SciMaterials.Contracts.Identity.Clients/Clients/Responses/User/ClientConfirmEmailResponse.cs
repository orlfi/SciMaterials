namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

public class ClientConfirmEmailResponse : Result.Result
{
    public string? Message { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}