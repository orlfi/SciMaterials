namespace SciMaterials.DAL.Contracts.Configuration;

public sealed class DatabaseSettings
{
    public string? Provider { get; init; }
    public bool RemoveAtStart { get; init; }
    public bool UseDataSeeder { get; init; }

    public string GetProviderName() => 
        Provider?.Split(".", 2, StringSplitOptions.RemoveEmptyEntries)[0] 
        ?? throw new ArgumentNullException(string.Empty, "Provider not set in configuration file");
}