using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace SciMaterials.DAL.AUTH.InitializationDb;

public static class AuthRolesInitializer
{
    /// <summary>
    /// Инициализация базы данных с созданием ролей "супер админ" и "пользователь"
    /// Создание одной учетной записи "админа"
    /// </summary>
    /// <param name="UserManager"></param>
    /// <param name="RoleManager"></param>
    public static async Task InitializeAsync(
        UserManager<IdentityUser> UserManager,
        RoleManager<IdentityRole> RoleManager,
        IConfiguration Configuration)
    {
        async Task CheckRoleAsync(string RoleName)
        {
            if (await RoleManager.FindByNameAsync(RoleName) is null && 
                await RoleManager.CreateAsync(new(RoleName)) is { Succeeded: false, Errors: var errors })
                throw new InvalidOperationException(
                    $"Ошибка создания роли {RoleName}: {string.Join(",", errors.Select(e => e.Description))}");
        }

        await CheckRoleAsync("admin").ConfigureAwait(false);
        await CheckRoleAsync("user");

        var admin_settings = Configuration.GetSection("AuthApiSettings:AdminSettings");
        var admin_email    = admin_settings["login"];
        var admin_password = admin_settings["password"];

        //Супер админ
        if (await UserManager.FindByNameAsync(admin_email) is null)
        {
            var super_admin = new IdentityUser
            {
                Email = admin_email,
                UserName = admin_email
            };

            if (await UserManager.CreateAsync(super_admin, admin_password) is { Succeeded: false, Errors: var errors })
                throw new InvalidOperationException($"Ошибка создания администратора {string.Join(",", errors.Select(e => e.Description))}");

            await UserManager.AddToRoleAsync(super_admin, "admin");
            var token_for_admin = await UserManager.GenerateEmailConfirmationTokenAsync(super_admin);
            await UserManager.ConfirmEmailAsync(super_admin, token_for_admin);
        }
    }
}