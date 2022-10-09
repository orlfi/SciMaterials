using SciMaterials.Auth.Utilits;

namespace SciMaterials.Auth.Registrations;

public static class RegisterUtils
{
    public static IServiceCollection RegisterAuthUtils(this IServiceCollection services)
    {
        return services.AddSingleton<IAuthUtils, AuthUtils>();
    }
}