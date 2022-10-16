using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public interface IAccountsService
{
    List<AuthorityGroup> AuthorityGroupsList();
    List<UserInfo> UsersList();
    void ChangeAuthority(Guid userId, Guid authorityId);
    void Delete(Guid userId);
}