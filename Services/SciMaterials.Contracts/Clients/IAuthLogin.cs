namespace SciMaterials.Contracts.Clients;

public interface IAuthLogin<TResult, TRequest>
{
    Task<TResult> LoginUserAsync(TRequest loginUser);
    Task<TResult> LogoutUserAsync();
}