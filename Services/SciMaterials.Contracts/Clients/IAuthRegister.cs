namespace SciMaterials.Contracts.Clients;

public interface IAuthRegister<TResult, TRequest>
{
    Task<TResult> RegisterUserAsync(TRequest registerUser);
}