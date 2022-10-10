namespace SciMaterials.Contracts.API.Models;

public readonly struct FileSaveResult : IFileSaveResult
{
    public string Hash { get; }
    public long Size { get; }

    public FileSaveResult(string hash, long size)
        => (Hash, Size) = (hash, size);
}