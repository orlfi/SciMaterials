namespace SciMaterials.UI.BWASM.Models;

public class UserInfo
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string Authority { get; set; } = null!;
    public Guid AuthorityGroupId { get; set; }

    public static UserInfo Create(string userName, string email, string password, AuthorityGroup authority) =>
        new()
        {
            UserName = userName,
            Email = email,
            Password = password,
            Authority = authority.Name,
            AuthorityGroupId = authority.Id
        };
}