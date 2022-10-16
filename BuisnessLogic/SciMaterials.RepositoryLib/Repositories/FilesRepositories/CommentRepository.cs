
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
        _logger.LogTrace($"Логгер встроен в {nameof(CommentRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Comment entity)
    {
        _logger.LogInformation($"{nameof(CommentRepository.Add)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(CommentRepository.Add)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Comments.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Comment entity)
    {
        _logger.LogInformation($"{nameof(CommentRepository.AddAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(CommentRepository.AddAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.Comments.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(Comment entity)
    {
        _logger.LogInformation($"{nameof(CommentRepository.Delete)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(CommentRepository.Delete)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(Comment entity)
    {
        _logger.LogInformation($"{nameof(CommentRepository.DeleteAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(CommentRepository.DeleteAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogInformation($"{nameof(CommentRepository.Delete)}");

        var entityDb = _context.Comments.FirstOrDefault(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(CommentRepository.Delete)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Comments.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(CommentRepository.DeleteAsync)}");

        var entityDb = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(CommentRepository.DeleteAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Comments.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Comment>? GetAll(bool disableTracking = true, bool include = false)
    {
        IQueryable<Comment> query = _context.Comments.Where(c => !c.IsDeleted);
        
        if (include)
            query = query.Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<Comment>?> GetAllAsync(bool disableTracking = true, bool include = false)
    {
        IQueryable<Comment> query = _context.Comments.Where(c => !c.IsDeleted);

        if (include)
            query = query.Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Comment? GetById(Guid id, bool disableTracking = true, bool include = false)
    {
        IQueryable<Comment> query = _context.Comments
                .Where(c => c.Id == id && !c.IsDeleted);

        if (include)
            query = query.Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<Comment?> GetByIdAsync(Guid id, bool disableTracking = true, bool include = false)
    {
        IQueryable<Comment> query = _context.Comments
                .Where(c => c.Id == id && !c.IsDeleted);

        if (include)
            query = query.Include(c => c.File)
                .Include(c => c.FileGroup)
                .Include(c => c.Author);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Comment entity)
    {
        _logger.LogInformation($"{nameof(CommentRepository.Update)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(CommentRepository.Update)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = GetById(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(CommentRepository.Update)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.Comments.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Comment entity)
    {
        _logger.LogInformation($"{nameof(CommentRepository.UpdateAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(CommentRepository.UpdateAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = await GetByIdAsync(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(CommentRepository.UpdateAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.Comments.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<Comment?> GetByNameAsync(string name, bool disableTracking = true, bool include = false) => null;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public Comment? GetByName(string name, bool disableTracking = true, bool include = false) => null;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<Comment?> GetByHashAsync(string hash, bool disableTracking = true, bool include = false) => null;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public Comment? GetByHash(string hash, bool disableTracking = true, bool include = false) => null;

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