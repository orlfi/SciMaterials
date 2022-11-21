using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.DependencyInjection;

using SciMaterials.Contracts.Identity.API;

namespace SciMaterials.WebApi.Clients.Identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityClients(this IServiceCollection services, Uri apiAddress)
        {
            services.AddHttpClient<IIdentityClient, IdentityClient>("IdentityClient", c =>
            {
                c.BaseAddress = apiAddress;
            });
            return services;
        }
    }
}