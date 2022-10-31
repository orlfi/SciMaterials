namespace SciMaterials.Contracts.Auth;

public interface IAuthUtils<in TUser, TRole>
{
    string CreateSessionToken(TUser User, IList<string> Roles);
    bool CheckToDeleteAdminOrUserRoles(TRole Role);
    bool CheckToDeleteSAInRoleAdmin(TUser User, string Role);
    bool CheckToDeleteSA(TUser User);
}