namespace SciMaterials.DAL.AUTH.Interfaces;

public interface IAuthUtils<TUser>
{
    string CreateSessionToken(TUser user, IList<string> role);
}