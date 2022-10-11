using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Services.Files;

namespace SciMaterials.Services.API.Services.Files.Stores;

public class FileSystemStore : IFileStore
{
    private const long _bufferSize = 100 * 1024 * 1024;
    private readonly ILogger<FileSystemStore> _logger;

    public FileSystemStore(ILogger<FileSystemStore> logger)
    {
        _logger = logger;
    }

    public async Task<FileMetadata> WriteAsync(string path, Stream stream, FileMetadata metadata, CancellationToken cancellationToken = default)
    {
        try
        {
            CreateDirectoryFromPath(path);
            (string Hash, long Size) writeResult = await WriteFileAsync(path, stream, cancellationToken);
            metadata.Hash = writeResult.Hash;
            metadata.Size = writeResult.Size;

            await WriteMetadataAsync(path, metadata, cancellationToken);
            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Write file error.");
            throw;
        }
    }

    private void CreateDirectoryFromPath(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(directory))
            throw new DirectoryNotFoundException();

        if (!Directory.Exists(directory))
        {
            _logger.LogDebug("Create directory {directory}", directory);
            Directory.CreateDirectory(directory);
        }
    }

    public Stream OpenRead(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"File {Path.GetFileName(path)}  not found");

        var stream = File.OpenRead(path);
        return stream;
    }

    public async Task<T> ReadMetadataAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Metadata file {Path.GetFileName(path)}  not found");

        using var reader = File.OpenRead(path);
        var metaData = await JsonSerializer.DeserializeAsync<T>(reader).ConfigureAwait(false);

        if (metaData is null)
            throw new NullReferenceException("Metadata cannot be read");

        return metaData;
    }

    public void Delete(string filePath)
    {
        File.Delete(filePath);

        File.Delete(GetMetadataPath(filePath));
    }

    private async Task<(string Hash, long Size)> WriteFileAsync(string filePath, Stream stream, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Write file {path} started...", filePath);
        var buffer = new byte[_bufferSize];
        int bytesRead;
        long totalBytesRead = 0;
        using var writeStream = File.Create(filePath);
        using var hashAlgorithm = SHA512.Create();
        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
        {
            await writeStream.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
            hashAlgorithm.TransformBlock(buffer, 0, bytesRead, null, 0);
            totalBytesRead += bytesRead;
        }
        hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
        var hash = hashAlgorithm.Hash;

        if (hash is null)
        {
            var exception = new NullReferenceException("Unable to calculate Hash");
            _logger.LogError(exception, null);
            throw exception;
        }

        var hashString = Convert.ToHexString(hash);
        _logger.LogDebug("The file was saved successfully.");
        return (hashString, totalBytesRead);
    }

    private async Task WriteMetadataAsync(string path, FileMetadata metadata, CancellationToken cancellationToken = default)
    {
        var metadaPath = GetMetadataPath(path);
        _logger.LogDebug("Write file metadata {metadaPath} started...", path);

        var metaData = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadaPath, metaData, cancellationToken).ConfigureAwait(false);
        _logger.LogDebug("The filemetadata was saved successfully.");
    }

    private static string GetMetadataPath(string filePath)
        => Path.ChangeExtension(filePath, "json");

}