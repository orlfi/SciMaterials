namespace SciMaterials.Contracts.Auth;

public interface IAuthUtils<TUser>
{
    string CreateSessionToken(TUser user, IList<string> role);
}