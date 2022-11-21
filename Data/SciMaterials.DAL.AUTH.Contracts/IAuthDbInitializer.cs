namespace SciMaterials.DAL.AUTH.Contracts;

public interface IAuthDbInitializer
{
    Task InitializeAsync(bool RemoveAtStart = false, CancellationToken Cancel = default);
}