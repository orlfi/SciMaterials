#region usings
using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog.LayoutRenderers;
using SciMaterials.ConsoleTests;
using SciMaterials.Contracts.API.DTO.Comments;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.InitializationDb.Implementation;
using SciMaterials.DAL.InitializationDb.Interfaces;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Services.API.Extensions;
using File = SciMaterials.DAL.Models.File;
#endregion

internal class Program
{
    private static async Task Main(string[] args)
    {
        const string path = @"d:\tmp\test.txt";

        static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureServices(ConfigureServices)
            .ConfigureAppConfiguration(app => app.AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>());

        static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddContextMultipleProviders(context);
            services.AddTransient<IDbInitializer, DbInitializer>();
            services.AddApiServices(context.Configuration);
        }

        using IHost host = CreateHostBuilder(args).Build();

        // Моделируем получение запроса на изменение
        Comment changedComment;
        using (var scope = host.Services.CreateAsyncScope())
        {
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            var context = scope.ServiceProvider.GetRequiredService<SciMaterialsContext>();
            var comment = context.Comments.First();
            var commentEditRequest = mapper.Map<CommentEditRequest>(comment);
            commentEditRequest.Text = "Измененный комментарий " + Random.Shared.Next(1, 10000).ToString();
            changedComment = mapper.Map<Comment>(commentEditRequest);
            Console.WriteLine($"Comment id {changedComment.Id}: {commentEditRequest.Text}");
        }

        UpdateWithPavelAlg(host, changedComment);
        //UpdateWithRepo(host, changedComment);
        // UpdateWithContext(host, changedComment);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
    static void UpdateWithRepo(IHost host, Comment changedComment)
    {
        Console.WriteLine($"Change comment text: {changedComment.Text}");
        using var scope = host.Services.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<SciMaterialsContext>>();

        unitOfWork.GetRepository<Comment>().Update(changedComment);
        unitOfWork.SaveContext();

        var commentDb = unitOfWork.GetRepository<Comment>().GetById(changedComment.Id);
        Console.WriteLine(commentDb is null ? "Почему то удалилась!!!!" : $"DB comment text: {commentDb.Text}");
    }

    static void UpdateWithPavelAlg(IHost host, Comment entity)
    {
        Console.WriteLine($"Change comment text: {entity.Text}");
        using var scope = host.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<SciMaterialsContext>();


        // _logger.LogInformation($"{nameof(CommentRepository.UpdateAsync)}");

        if (entity is null)
        {
            // _logger.LogError($"{nameof(CommentRepository.UpdateAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        // var entityDb = await context.GetByIdAsync(entity.Id, false);
        IQueryable<Comment> query = context.Comments
                .Where(c => c.Id == entity.Id);
        // .Include(c => c.File)
        // .Include(c => c.FileGroup)
        // .Include(c => c.Author);

        // Здесь ошибка !!!!!!!!!!!!!! Если включить отслеживание и с Include'ами, то удаляется!!!!!!!!!!!!!!!!
        // query = query.AsNoTracking(); 

        var entityDb = query.FirstOrDefault();

        if (entityDb is null)
        {
            // _logger.LogdError($"{nameof(CommentRepository.UpdateAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        context.Comments.Update(entityDb);
        context.SaveChanges();

        var commentDb = context.Comments.Find(entity.Id); ;
        Console.WriteLine(commentDb is null ? "Почему то удалилась!!!!" : $"DB comment text: {commentDb.Text}");
    }

    private static Comment UpdateCurrentEntity(Comment sourse, Comment recipient)
    {
        recipient.CreatedAt = sourse.CreatedAt;
        recipient.FileId = sourse.FileId;
        recipient.File = sourse.File;
        recipient.ParentId = sourse.ParentId;
        recipient.Text = sourse.Text;
        recipient.FileGroupId = sourse.FileGroupId;
        recipient.FileGroup = sourse.FileGroup;
        recipient.Author = sourse.Author;
        recipient.AuthorId = sourse.AuthorId;

        return recipient;
    }

    static void UpdateWithContext(IHost host, Comment changedComment)
    {
        using var scope = host.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<SciMaterialsContext>();
        try
        {
            var exist = context.Comments.Find(changedComment.Id);
            Console.WriteLine($"DB comment text: {exist.Text} >> {changedComment.Text}");
            context.Entry(exist).CurrentValues.SetValues(changedComment);
            context.SaveChanges();
            var commentDb = context.Comments.Find(changedComment.Id);
            Console.WriteLine($"DB comment text: {commentDb.Text}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message} >>>  {ex.InnerException}");
        }
    }

    static async Task<Result<Guid>> SendFile(string path, AddEditFileRequest uploadRequest)
    {

        using HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://localhost:5185/")
        };

        var fileClient = new FilesClient(httpClient);

        var payload = System.Text.Json.JsonSerializer.Serialize(uploadRequest);
        var result = await fileClient.UploadAsync(path, payload);
        return result;
    }
}


// Console.WriteLine($"comment id <{commentDb.Id}>\t file group: {commentDb.FileGroupId}");
// // var fileGroup = (await unitOfWork.GetRepository<FileGroup>().GetAllAsync()).Last();
// // var fileGroup = context.Set<FileGroup>().Last();

// Comment edited = new Comment()
// {
//     Id = comment.Id,
//     Text = comment.Text,
//     AuthorId = comment.AuthorId,
//     CreatedAt = comment.CreatedAt,
//     FileId = comment.FileId,
//     FileGroupId = comment.FileGroupId
// };
// await unitOfWork.GetRepository<Comment>().UpdateAsync(edited);
// var saveResult = await unitOfWork.SaveContextAsync();
// var updatetedComment = await unitOfWork.GetRepository<Comment>().GetByIdAsync(edited.Id);

// Console.WriteLine($"SaveResult:{saveResult} \tcomment id <{updatetedComment?.Id.ToString() ?? "Не найдена"}>");
