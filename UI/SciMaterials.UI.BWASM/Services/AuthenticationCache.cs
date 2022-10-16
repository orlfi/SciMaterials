using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using SciMaterials.Contracts.Result;
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

    public List<AuthorityGroup> AuthorityGroupsList()
    {
        return AuthorityGroups.Values.ToList();
    }

    public List<UserInfo> UsersList()
    {
        return Users.Values.ToList();
    }

    public Result ChangeAuthorityGroup(Guid userId, Guid authorityGroupId)
    {
        if (AuthorityGroups.Values.FirstOrDefault(x => x.Id == authorityGroupId) is not { } authorityGroup)
        {
            // just random
            return Result.Error(242);
        }

        if (!Users.TryGetValue(userId, out var user))
        {
            return Result.Error(243);
        }

        user.Authority = authorityGroup.Name;
        user.AuthorityGroupId = authorityGroupId;
        return Result.Success();
    }

    public Result DeleteUser(Guid userId)
    {
        return !Users.TryRemove(userId, out _) 
            ? Result.Error(243) 
            : Result.Success();
    }

    public List<Authority> AuthoritiesList()
    {
        return Authorities.Values.ToList();
    }

    public void DeleteAuthority(Guid authorityId)
    {
        if (!Authorities.TryRemove(authorityId, out var existedAuthority)) return;

        foreach (var authorityGroup in AuthorityGroups.Values)
        {
            authorityGroup.Authorities.Remove(existedAuthority);
        }
    }

    public void DeleteAuthorityGroup(Guid authorityGroupId, string authorityGroupName)
    {

        if(authorityGroupName is "User" or "Admin" || !AuthorityGroups.TryRemove(authorityGroupName, out _)) return;

        var userGroup = AuthorityGroups["User"];

        foreach (var user in Users.Values.Where(x=>x.AuthorityGroupId == authorityGroupId))
        {
            user.AuthorityGroupId = userGroup.Id;
            user.Authority = userGroup.Name;
        }
    }

    public void AddAuthorityToGroup(Guid groupId, string groupName, Guid authorityId)
    {
        if(!AuthorityGroups.TryGetValue(groupName, out var group) || !Authorities.TryGetValue(authorityId, out var authority)) return;
        // check that authority already exist
        if(group.Authorities.Contains(authority)) return;
        group.Authorities.Add(authority);
    }

    public void AddAuthority(string authorityName)
    {
        if(Authorities.Values.FirstOrDefault(x=>x.Name == authorityName) is not null) return;

        Authority newOne = Authority.Create(authorityName);
        Authorities.TryAdd(newOne.Id, newOne);
    }
}