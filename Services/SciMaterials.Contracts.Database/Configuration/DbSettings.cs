#nullable disable
namespace SciMaterials.Contracts.Database.Configuration;

public sealed class DbSettings
{
    public string DbProvider { get; init; }
    public bool RemoveAtStart { get; init; }
    public bool UseDataSeeder { get; init; }
}