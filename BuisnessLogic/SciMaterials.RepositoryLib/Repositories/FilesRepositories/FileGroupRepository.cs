
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.Repositories.FilesRepositories;

/// <summary> Интерфейс репозитория для <see cref="FileGroup"/>. </summary>
public interface IFileGroupRepository : IRepository<FileGroup> { }

/// <summary> Репозиторий для <see cref="FileGroup"/>. </summary>
public class FileGroupRepository : IFileGroupRepository
{
    private readonly ILogger _logger;
    private readonly ISciMaterialsContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public FileGroupRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogTrace($"Логгер встроен в {nameof(FileGroupRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(FileGroup entity)
    {
        _logger.LogInformation($"{nameof(FileGroupRepository.Add)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileGroupRepository.Add)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.FileGroups.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(FileGroup entity)
    {
        _logger.LogInformation($"{nameof(FileGroupRepository.AddAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileGroupRepository.AddAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.FileGroups.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(FileGroup entity)
    {
        _logger.LogInformation($"{nameof(FileGroupRepository.Delete)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileGroupRepository.Delete)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(FileGroup entity)
    {
        _logger.LogInformation($"{nameof(FileGroupRepository.DeleteAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileGroupRepository.DeleteAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogInformation($"{nameof(FileGroupRepository.Delete)}");

        var entityDb = _context.FileGroups.FirstOrDefault(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(FileGroupRepository.Delete)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.FileGroups.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(FileGroupRepository.DeleteAsync)}");

        var entityDb = await _context.FileGroups.FirstOrDefaultAsync(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(FileGroupRepository.DeleteAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.FileGroups.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<FileGroup>? GetAll(bool disableTracking = true, bool include = false)
    {
        IQueryable<FileGroup> query = _context.FileGroups.Where(f => !f.IsDeleted);

        if (include)
            query.Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<FileGroup>?> GetAllAsync(bool disableTracking = true, bool include = false)
    {
        IQueryable<FileGroup> query = _context.FileGroups.Where(f => !f.IsDeleted);

        if (include)
            query.Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public FileGroup? GetById(Guid id, bool disableTracking = true, bool include = false)
    {
        IQueryable<FileGroup> query = _context.FileGroups
                .Where(c => c.Id == id && !c.IsDeleted);

        if (include)
            query.Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<FileGroup?> GetByIdAsync(Guid id, bool disableTracking = true, bool include = false)
    {
        IQueryable<FileGroup> query = _context.FileGroups
                .Where(c => c.Id == id && !c.IsDeleted);

        if (include)
            query.Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(FileGroup entity)
    {
        _logger.LogInformation($"{nameof(FileGroupRepository.Update)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileGroupRepository.Update)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = GetById(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(FileGroupRepository.Update)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }


        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.FileGroups.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(FileGroup entity)
    {
        _logger.LogInformation($"{nameof(FileGroupRepository.UpdateAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileGroupRepository.UpdateAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = await GetByIdAsync(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(FileGroupRepository.UpdateAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }


        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.FileGroups.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<FileGroup?> GetByNameAsync(string name, bool disableTracking = true, bool include = false)
    {
        IQueryable<FileGroup> query = _context.FileGroups
                .Where(c => c.Name == name && !c.IsDeleted);

        if (include)
            query.Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public FileGroup? GetByName(string name, bool disableTracking = true, bool include = false)
    {
        IQueryable<FileGroup> query = _context.FileGroups
                .Where(c => c.Name == name && !c.IsDeleted);

        if (include)
            query.Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<FileGroup?> GetByHashAsync(string hash, bool disableTracking = true, bool include = false) => null;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public FileGroup? GetByHash(string hash, bool disableTracking = true, bool include = false) => null;

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private FileGroup UpdateCurrentEntity(FileGroup sourse, FileGroup recipient)
    {
        recipient.Name = sourse.Name;

        recipient.Title = sourse.Title;
        recipient.Description = sourse.Description;
        recipient.AuthorId = sourse.AuthorId;
        recipient.CreatedAt = sourse.CreatedAt;
        recipient.Author = sourse.Author;
        recipient.Comments = sourse.Comments;
        recipient.Tags = sourse.Tags;
        recipient.Categories = sourse.Categories;
        recipient.Ratings = sourse.Ratings;

        recipient.Files = sourse.Files;

        return recipient;
    }
}