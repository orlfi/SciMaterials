#region usings
// using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.InitializationDb.Implementation;
using SciMaterials.DAL.InitializationDb.Interfaces;
using SciMaterials.DAL.Models;
#endregion

static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
    .ConfigureServices(ConfigureServices);


static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    var defaultProvider = context.Configuration.GetValue<string>("DefaultProvider");
    services.AddDbContext<SciMaterialsContext>(options => _ = defaultProvider switch
    {
        "SqlServer" => options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"),
            x => x.MigrationsAssembly("SciMaterials.MsSqlServerMigrations")),
        "PostgreSql" => options.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=SciMaterials;Integrated Security=True",
                x => x.MigrationsAssembly("SciMaterials.MsSqlServerMigrations")),
        _ => throw new Exception("Не задана БД по умолчанию")
    });
    services.AddTransient<IDbInitializer, DbInitializer>();
}

using IHost host = CreateHostBuilder(args).Build();

await using (var scope = host.Services.CreateAsyncScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await dbInitializer.InitializeDbAsync();
    
    var db = scope.ServiceProvider.GetRequiredService<SciMaterialsContext>();

    var category = new Category
    {
        Id = Guid.NewGuid(),
        Name = "Books",
        CreatedAt = DateTime.Now,
    };
    await db.Categories.AddAsync(category);

    var user = new User
    {
        Id = Guid.NewGuid(),
        Name = "Иванов Иван",
    };
    await db.Users.AddAsync(user);

    var file = new SciMaterials.DAL.Models.File
    {
        Id = Guid.NewGuid(),
        Name = "test.txt",
        Owner = user,
        Title = "Тестовый файл",
        Category = category,
        CreatedAt = DateTime.Now,
    };
    await db.Files.AddAsync(file);

    var fileGroup = new FileGroup
    {
        Id = Guid.NewGuid(),
        Name = "TestGroup",
        Owner = user,
        Title = "Тестовая группа",
        Category = category,
        CreatedAt = DateTime.Now,
    };
    await db.FileGroups.AddAsync(fileGroup);

    var fileComment = new Comment
    {
        Id = Guid.NewGuid(),
        Text = "Пример комментария File",
        OwnerId = file.OwnerId,
        CreatedAt = DateTime.Now,
        File = file,
    };

    var groupComment = new Comment
    {
        Id = Guid.NewGuid(),
        Text = "Пример комментария FileGroup",
        OwnerId = file.OwnerId,
        CreatedAt = DateTime.Now,
        FileGroup = fileGroup,
    };

    file.Comments.Add(fileComment);
    fileGroup.Comments.Add(groupComment);

    await db.SaveChangesAsync();
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();