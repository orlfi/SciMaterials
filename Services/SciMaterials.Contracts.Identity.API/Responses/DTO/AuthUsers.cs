namespace SciMaterials.Contracts.Identity.API.Responses.DTO
{
    public class AuthUsers
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public List<AuthRoles>? UserRoles { get; set; }
    }
}