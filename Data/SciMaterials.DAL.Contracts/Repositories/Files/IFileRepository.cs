using File = SciMaterials.DAL.Contracts.Entities.File;

namespace SciMaterials.DAL.Contracts.Repositories.Files
{
    /// <summary> Интерфейс репозитория для <see cref="File"/>. </summary>
    public interface IFileRepository : IRepository<File> { }
}