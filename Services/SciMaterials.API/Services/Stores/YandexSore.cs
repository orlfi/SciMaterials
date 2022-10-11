<<<<<<<< HEAD:Services/SciMaterials.API/Services/Stores/YandexSore.cs
using System.Security.Cryptography;
using System.Text.Json;
using SciMaterials.API.Models;
using SciMaterials.API.Services.Interfaces;

namespace SciMaterials.API.Services.Stores;

public class YandexSore : IFileStore
{
    public void Delete(string path)
    {
        throw new NotImplementedException();
    }

    public Stream OpenRead(string path)
    {
        throw new NotImplementedException();
    }

    public Task<T> ReadMetadataAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<FileSaveResult> WriteAsync(string path, Stream stream, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task WriteMetadataAsync<T>(string path, T data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
========
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Services.Files;

namespace SciMaterials.Services.API.Services.Files.Stores;

public class YandexSore : IFileStore
{
    public void Delete(string path)
    {
        throw new NotImplementedException();
    }

    public Stream OpenRead(string path)
    {
        throw new NotImplementedException();
    }

    public Task<T> ReadMetadataAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<FileMetadata> WriteAsync(string path, Stream stream, FileMetadata metadata, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task WriteMetadataAsync<T>(string path, T data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
>>>>>>>> a2bf4784273b79ee5d485e0c093be5b070da692c:Services/SciMaterials.Services.API/Services/Files/Stores/YandexSore.cs
}