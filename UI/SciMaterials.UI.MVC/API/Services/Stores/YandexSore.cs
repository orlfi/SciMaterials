using SciMaterials.UI.MVC.API.Models;
using SciMaterials.UI.MVC.API.Services.Interfaces;

namespace SciMaterials.UI.MVC.API.Services.Stores;

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
}