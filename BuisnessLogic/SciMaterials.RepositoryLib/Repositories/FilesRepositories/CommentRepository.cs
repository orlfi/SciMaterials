
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Repositories.CategorysRepositories;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.Repositories.CommentsRepositories;

/// <summary> Интерфейс репозитория для <see cref="Comment"/>. </summary>
public interface ICommentRepository : IRepository<Comment> { }

/// <summary> Репозиторий для <see cref="Comment"/>. </summary>
public class CommentRepository : ICommentRepository
{
    private readonly ILogger _logger;
    private readonly ISciMaterialsContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public CommentRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogDebug($"Логгер встроен в {nameof(CommentRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Comment entity)
    {
        _logger.LogDebug($"{nameof(CommentRepository.Add)}");

        if (entity is null) return;
        _context.Comments.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Comment entity)
    {
        _logger.LogDebug($"{nameof(CommentRepository.AddAsync)}");

        if (entity is null) return;
        await _context.Comments.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(Comment entity)
    {
        _logger.LogDebug($"{nameof(CommentRepository.Delete)}");
        if (entity is null || entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(Comment entity)
    {
        _logger.LogDebug($"{nameof(CommentRepository.DeleteAsync)}");
        if (entity is null || entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogDebug($"{nameof(CommentRepository.Delete)}");

        var categoryDb = _context.Comments.FirstOrDefault(c => c.Id == id);
        if (categoryDb is null) return;
        _context.Comments.Remove(categoryDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug($"{nameof(CommentRepository.DeleteAsync)}");

        var categoryDb = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (categoryDb is null) return;
        _context.Comments.Remove(categoryDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Comment>? GetAll(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CommentRepository.GetAll)}");

        if (disableTracking)
            return _context.Comments
                .Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author)
                .AsNoTracking()
                .ToList();
        else
            return _context.Comments
                .Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author)
                .ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<Comment>?> GetAllAsync(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CommentRepository.GetAllAsync)}");

        if (disableTracking)
            return await _context.Comments
                .Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author)
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.Comments
                .Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author)
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Comment GetById(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CommentRepository.GetById)}");

        if (disableTracking)
            return _context.Comments
                .Where(c => c.Id == id)
                .Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Comments
                .Where(c => c.Id == id)
                .Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<Comment?> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CommentRepository.GetByIdAsync)}");

        if (disableTracking)
            return (await _context.Comments
                .Where(c => c.Id == id)
                .Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Comments
                .Where(c => c.Id == id)
                .Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Comment entity)
    {
        _logger.LogDebug($"{nameof(CommentRepository.Update)}");

        if (entity is null) return;
        var categoryDb = GetById(entity.Id, false);

        categoryDb = UpdateCurrentEntity(entity, categoryDb);
        _context.Comments.Update(categoryDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Comment entity)
    {
        _logger.LogDebug($"{nameof(CommentRepository.UpdateAsync)}");

        _logger.LogDebug($"{nameof(CommentRepository.UpdateAsync)}");

        if (entity is null) return;
        var categoryDb = await GetByIdAsync(entity.Id, false);

        categoryDb = UpdateCurrentEntity(entity, categoryDb!);
        _context.Comments.Update(categoryDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<Comment?> GetByNameAsync(string name, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CommentRepository.GetByNameAsync)}");

        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public Comment? GetByName(string name, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CommentRepository.GetByName)}");

        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<Comment?> GetByHashAsync(string hash, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CommentRepository.GetByHashAsync)}");
        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public Comment? GetByHash(string hash, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CommentRepository.GetByHash)}");
        return null!;
    }

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private Comment UpdateCurrentEntity(Comment sourse, Comment recipient)
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
}