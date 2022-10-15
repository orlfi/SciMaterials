#nullable disable
namespace SciMaterials.Services.Database.Configuration
{
    public sealed class DbSettings
    {
        public string DbProvider { get; set; }
        public bool RemoveAtStart { get; set; }
        public bool UseDataSeeder { get; set; }
    }
}
