using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using SciMaterials.Contracts.WebAPI.LinkSearch;

namespace SciMaterials.WebAPI.LinkSearch.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiLinkSearch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILinkSearch, LinkSearch>();
            return services;
        }
    }
}