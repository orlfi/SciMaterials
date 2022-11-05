using System.ComponentModel;

namespace SciMaterials.Services.Database.Enums
{
    public enum Migrations : byte
    {
        [Description("SciMaterials.MsSqlServerMigrations")]
        SqlServer,

        [Description("SciMaterials.PostgresqlMigrations")]
        PostgreSQL,

        [Description("SciMaterials.Data.MySqlMigrations")]
        MySQL,

        [Description("SciMaterials.SQLiteMigrations")]
        SQLite
    }
}
