using Microsoft.Extensions.Configuration;

namespace SciMaterials.DAL.AUTH.Interfaces;

public interface IAuthDbInitializer
{
    Task InitializeAsync(IConfiguration configuration, CancellationToken cancellationToken = default);
}