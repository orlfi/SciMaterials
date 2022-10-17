namespace SciMaterials.Contracts.Auth;

public interface IAuthDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}