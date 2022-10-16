using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public interface IAuthoritiesService
{
    List<AuthorityGroup> AuthoritiesGroupsList();
    List<Authority> AuthoritiesList();
}