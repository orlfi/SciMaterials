using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public class AuthenticationCache
{
    private ConcurrentDictionary<Guid, Authority> Authorities { get; }
    private ConcurrentDictionary<string, AuthorityGroup> AuthorityGroups { get; } = new();
    private ConcurrentDictionary<Guid, UserInfo> Users { get; } = new();

    public AuthenticationCache()
    {
        Authority[] adminAuthorities =
        {
            Authority.CanAccessUsers,
            Authority.CanChangeUsersAuthority,
            Authority.CanDeleteUsers
        };

        AuthorityGroup adminAuthorityGroup = AuthorityGroup.Create("Admin", adminAuthorities);
        AuthorityGroup userAuthorityGroup = AuthorityGroup.Create("User");

        Authorities = new(adminAuthorities.ToDictionary(e=>e.Id));
        AuthorityGroups.TryAdd(adminAuthorityGroup.Name, adminAuthorityGroup);
        AuthorityGroups.TryAdd(userAuthorityGroup.Name, userAuthorityGroup);

        UserInfo admin = UserInfo.Create("Admin", "sa@mail.ru", "test12345", adminAuthorityGroup);
        Users.TryAdd(admin.Id, admin);
    }

    public bool TryAdd(string email, string password, string userName)
    {
        if (Users.Values.FirstOrDefault(x => x.Email == email) is not null) return false;

        UserInfo user = UserInfo.Create(userName, email, password, AuthorityGroups["User"]);

        // no check on idempotency
        Users.TryAdd(user.Id, user);

        return true;
    }

    public bool TryGetIdentity(string email, string password,[NotNullWhen(true)] out ClaimsIdentity? claimsIdentity, out Guid userId)
    {
        userId = Guid.Empty;
        claimsIdentity = null;
        if(Users.Values.FirstOrDefault(x => x.Email == email && x.Password == password) is not {} user) return false;

        claimsIdentity = GenerateIdentity(user);
        userId = user.Id;
        return true;
    }

    public bool TryGetIdentity(Guid userId, [NotNullWhen(true)] out ClaimsIdentity? identity)
    {
        identity = null;
        if (!Users.TryGetValue(userId, out var user)) return false;
        identity = GenerateIdentity(user);
        return true;
    }

    private static ClaimsIdentity GenerateIdentity(UserInfo user) => new(new Claim[]
    {
        new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Role, user.Authority),
    }, "InMemoryScheme");
}