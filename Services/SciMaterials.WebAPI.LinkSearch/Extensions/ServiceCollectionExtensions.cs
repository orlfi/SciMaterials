using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using SciMaterials.Contracts.WebAPI.LinkSearch;
using SciMaterials.DAL.Models;
using SciMaterials.WebAPI.LinkSearch.Options;

namespace SciMaterials.WebAPI.LinkSearch.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiLinkSearch(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<ILinkSearch, LinkSearch>();
            services.AddScoped<ILinkShortCut<Link>, LinkShortCut>();
            services
                .AddOptions<LinkShortCutOptions>()
                .Validate(options => options.HashAlgorithm is "SHA512")
                .Validate(options => options.Encoding is "UTF-8" or "UTF-32" or "Unicode" or "ASCII");

            return services;
        }
    }
}