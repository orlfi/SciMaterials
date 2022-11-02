using SciMaterials.Contracts.Result;
using SciMaterials.DAL.Models;

namespace SciMaterials.Contracts.ShortLinks;

public interface ILinkShortCutService
{
    Task<Result<string>> GetOrAddAsync(string sourceAddress, CancellationToken Cancel = default);
}