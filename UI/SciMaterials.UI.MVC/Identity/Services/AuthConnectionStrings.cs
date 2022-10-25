using Microsoft.Data.Sqlite;

using MySqlConnector;

namespace SciMaterials.UI.MVC.Identity.Services;

/// <summary>
/// Статический класс по формированию строк подключения к БД
/// </summary>
public static class AuthConnectionStrings
{
    /// <summary>
    /// Метод формирует строку подключения для БД MYSQL
    /// </summary>
    /// <param name="configuration">Конфигурация</param>
    /// <returns>Сформированную строку подключения</returns>
    public static string MySql(IConfiguration configuration)
    {
        var connection_string = configuration.GetConnectionString("AuthDbConnection");
        var db_config = configuration.GetSection("AuthApiSettings:MySqlDbConfig");
        var builder = new MySqlConnectionStringBuilder(connection_string);
        builder.Server = db_config["server"];
        builder.Database = db_config["database"];
        builder.Port = Convert.ToUInt32(db_config["port"]);
        builder.UserID = db_config["userid"];
        builder.Password = db_config["password"];
        connection_string = builder.ConnectionString;

        return connection_string;
    }

    /// <summary>
    /// Метод формирует строку подключения для БД SQLITE
    /// </summary>
    /// <param name="configuration">Конфигурация</param>
    /// <returns>Сформированную строку подключения</returns>
    public static string Sqlite(IConfiguration configuration)
    {
        var connection_string = configuration.GetConnectionString("AuthDbConnection");
        var db_config = configuration.GetSection("AuthApiSettings:SqliteDbConfig");
        var builder = new SqliteConnectionStringBuilder(connection_string);
        builder.DataSource = db_config["datasource"];
        connection_string = builder.ConnectionString;

        return connection_string;
    }
}