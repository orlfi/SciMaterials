namespace SciMaterials.Contracts.API.Constants;

/// <summary>
/// Маршруты AuthApi
/// </summary>
public static class AuthApiRoute
{
    /// <summary>
    /// Uri адрес сервиса
    /// </summary>
    public const string AuthApiUri = "https://localhost:7183";
    
    /// <summary>
    /// Контроллер аутентификации
    /// </summary>
    public const string AuthControllerName = "auth/";

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    public const string Register = "register";
    
    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    public const string Login = "login";
    
    /// <summary>
    /// Выход из системы
    /// </summary>
    public const string Logout = "logout";
    
    /// <summary>
    /// Смена пароля пользователя
    /// </summary>
    public const string ChangePassword = "changepassword";
    
    /// <summary>
    /// Подтверждение почты при регистрации
    /// </summary>
    public const string ConfirmEmail = "confirmemail";
    
    /// <summary>
    /// Создание роли пользователя
    /// </summary>
    public const string CreateRole = "createrole";
    
    /// <summary>
    /// Получение всех ролей в системе
    /// </summary>
    public const string GetAllRoles = "getallroles";
    
    /// <summary>
    /// Получение роли по идентификатору
    /// </summary>
    public const string GetRoleById = "getrolebyid";

    /// <summary>
    /// Изменение названия роли по идентификатору роли
    /// </summary>
    public const string EditRoleById = "editrolebyid";
    
    /// <summary>
    /// Удаление роли по идентификатору
    /// </summary>
    public const string DeleteRoleById = "deleterolebyid";
    
    /// <summary>
    /// Добавление роли к пользователю
    /// </summary>
    public const string AddRoleToUser = "addroletouser";
    
    /// <summary>
    /// Удаление роли у пользователя
    /// </summary>
    public const string DeleteUserRole = "deleteuserrole";
    
    /// <summary>
    /// Получение списка всех ролей у конкретного пользователя
    /// </summary>
    public const string ListOfUserRoles = "listofuserroles";
    
    /// <summary>
    /// Создание пользователя админом
    /// </summary>
    public const string CreateUser = "createuser";
    
    /// <summary>
    /// Получение инф. о пользовател по email
    /// </summary>
    public const string GetUserByEmail = "getuserbyemail";
    
    /// <summary>
    /// Получение инф. о всех пользователях в системе
    /// </summary>
    public const string GetAllUsers = "getallusers";
    
    /// <summary>
    /// Редактирование инф. о пользователе по его email
    /// </summary>
    public const string EditUserByEmail = "edituserbyemail";
    
    /// <summary>
    /// Удаление пользователя по email
    /// </summary>
    public const string DeleteUserByEmail = "deleteuserbyemail";
    
    /// <summary>
    /// Удаление всех не подтвердивших свою почту пользователей в системе (для чистки БД админом)
    /// </summary>
    public const string DeleteUserWithoutConfirmation = "deleteuserwithoutconfirmation";
}