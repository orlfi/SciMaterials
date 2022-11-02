namespace SciMaterials.Contracts.Auth;

public interface IAuthDbInitializer
{
    Task InitializeAsync(bool RemoveAtStart = false, CancellationToken Cancel = default);
}