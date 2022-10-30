
namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.User
{
    public class ClientLoginResponse : Result.Result
    {
        public string? SessionToken { get; set; }
        public string? Message { get; set; }
        public int Code { get; set; }
        public bool Succeeded { get; set; }
    }
}