#nullable disable
namespace SciMaterials.Contracts.Database.Configuration;

public sealed class DbSettings
{
    public string DbProvider { get; set; }
    public bool RemoveAtStart { get; set; }
    public bool UseDataSeeder { get; set; }
}