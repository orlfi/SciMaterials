using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public interface IAuthoritiesService
{
    List<AuthorityGroup> AuthoritiesGroupsList();
    List<Authority> AuthoritiesList();
    void Delete(AuthorityGroup authorityGroup);
    void Delete(Authority authority);
    void AddAuthorityToGroup(AuthorityGroup group, Authority authority);
    void AddAuthority(string authorityName);
}