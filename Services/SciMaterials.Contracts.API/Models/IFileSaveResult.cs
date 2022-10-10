namespace SciMaterials.Contracts.API.Models
{
    public interface IFileSaveResult
    {
        string Hash { get; }
        long Size { get; }
    }
}