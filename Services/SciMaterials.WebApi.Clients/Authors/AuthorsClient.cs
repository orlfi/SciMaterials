using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Authors;

namespace SciMaterials.WebApi.Clients.Authors;

public class AuthorsClient :
    ApiClientWithAddBase<AuthorsClient, AddAuthorRequest, EditAuthorRequest, Guid, GetAuthorResponse>,
    IAuthorsClient
{
    public AuthorsClient(HttpClient httpClient, ILogger<AuthorsClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Authors;
}
