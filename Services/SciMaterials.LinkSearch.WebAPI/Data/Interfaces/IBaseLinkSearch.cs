namespace SciMaterials.LinkSearch.WebAPI.Data.Interfaces
{
    public interface IBaseLinkSearch<T>
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancel = default);
        Task<T> GetByIdAsync(int id, CancellationToken cancel = default);
        Task CreateAsync(T data, CancellationToken cancel = default);
        Task DeleteAsync(int id, CancellationToken cancel = default);
    }
}