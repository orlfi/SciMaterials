using System.Collections.Concurrent;
using SciMaterials.UI.MVC.API.Models;
using SciMaterials.UI.MVC.API.Data.Interfaces;

namespace SciMaterials.UI.MVC.API.Data;

public class FileInfoMemoryRepository : IFileRepository<Guid>
{
    private readonly ConcurrentDictionary<Guid, FileModel> _files;

    public FileInfoMemoryRepository()
    {
        _files = new(GetFakeData());
    }

    private Dictionary<Guid, FileModel> GetFakeData()
    {
        var data = new List<FileModel>()
        {
            new (){ FileName = "Text.txt", Hash = "fagergerg", Size = 234234, Id =Guid.NewGuid()},
            new (){ FileName = "book.pdf", Hash = "fagergerg", Size = 4353453, Id =Guid.NewGuid()},
            new (){ FileName = "video.avi", Hash = "fagergerg", Size = 325453453, Id =Guid.NewGuid()},
        };
        return data.ToDictionary(i => i.Id);
    }

    public bool Add(FileModel model)
        => _files.TryAdd(model.Id, model);

    public void Update(FileModel model)
        => _files[model.Id] = model;

    public void AddOrUpdate(FileModel model)
    {
        _files.AddOrUpdate(
            model.Id,
            model,
            (id, fileInfo) => fileInfo = model);
    }

    public void Delete(Guid id)
        => _files.Remove(id, out _);

    public FileModel? GetByHash(string hash)
        => _files.Values.SingleOrDefault(item => item.Hash == hash);

    public FileModel? GetById(Guid id)
     => _files.GetValueOrDefault(id);

    public FileModel? GetByName(string fileName)
        => _files.Values.SingleOrDefault(item => item.FileName == fileName);

    public IEnumerable<FileModel> GetAll()
        => _files.Values;
}