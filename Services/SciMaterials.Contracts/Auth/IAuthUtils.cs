namespace SciMaterials.Contracts.Auth;

public interface IAuthUtils<TUser>
{
    string CreateSessionToken(TUser User, IList<string> Roles);
    bool CheckTokenIsEmptyOrInvalid(string token);
}