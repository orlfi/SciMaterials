using SciMaterials.UI.MVC.API.Models;

namespace SciMaterials.UI.MVC.API.Data.Interfaces;

public interface IFileRepository<T>
{
    IEnumerable<FileModel> GetAll();
    FileModel? GetByHash(string hash);
    FileModel? GetById(T id);
    FileModel? GetByName(string name);

    bool Add(FileModel model);
    void Update(FileModel model);
    void AddOrUpdate(FileModel model);
    void Delete(T id);
}
