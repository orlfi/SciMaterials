using SciMaterials.DAL.Models;

namespace SciMaterials.Contracts.WebAPI.LinkSearch
{
    public interface ILinkShortCut<T>
    {
        Task<string> AddAsync(string sourceAddress, CancellationToken cancel = default);
        Task<T> FindByHashAsync(string hash, CancellationToken cancel = default);
        Task<T> FindByUrlAsync(string sourceAddress, CancellationToken cancel = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancel = default);
        Task DeleteByHashAsync(string hash, CancellationToken cancel = default);
        Task DeleteByUrlAsync(string sourceAddress, CancellationToken cancel = default);
    }
}