namespace SciMaterials.Contracts.Identity.API.Responses.User
{
    public class ClientLoginResponse : Result.Result
    {
        public string? SessionToken { get; set; }
        public string? Message { get; set; }
        public int Code { get; set; }
        public bool Succeeded { get; set; }
    }
}