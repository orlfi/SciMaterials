using Microsoft.Data.Sqlite;
using MySqlConnector;

namespace SciMaterials.AUTH.Utilits;

public static class AuthConnectionStrings
{
    public static string MySql(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AuthDbConnection");
        var dbConfig = configuration.GetSection("AuthApiSettings:MySqlDbConfig");
        var builder = new MySqlConnectionStringBuilder(connectionString);
        builder.Server = dbConfig["server"];
        builder.Database = dbConfig["database"];
        builder.Port = Convert.ToUInt32(dbConfig["port"]);
        builder.UserID = dbConfig["userid"];
        builder.Password = dbConfig["password"];
        connectionString = builder.ConnectionString;

        return connectionString;
    }

    public static string Sqlite(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AuthDbConnection");
        var dbConfig = configuration.GetSection("AuthApiSettings:SqliteDbConfig");
        var builder = new SqliteConnectionStringBuilder(connectionString);
        builder.DataSource = dbConfig["datasource"];
        connectionString = builder.ConnectionString;

        return connectionString;
    }
}