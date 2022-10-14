using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.Repositories.FilesRepositories;

/// <summary> Интерфейс репозитория для <see cref="Tag"/>. </summary>
public interface ITagRepository : IRepository<Tag> { }

/// <summary> Репозиторий для <see cref="Tag"/>. </summary>
public class TagRepository : ITagRepository
{
    private readonly ILogger _logger;
    private readonly ISciMaterialsContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public TagRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogTrace($"Логгер встроен в {nameof(TagRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Tag entity)
    {
        _logger.LogInformation($"{nameof(TagRepository.Add)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(TagRepository.Add)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }
        _context.Tags.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Tag entity)
    {
        _logger.LogInformation($"{nameof(TagRepository.AddAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(TagRepository.AddAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }
        await _context.Tags.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(Tag entity)
    {
        _logger.LogInformation($"{nameof(TagRepository.Delete)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(TagRepository.Delete)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(Tag entity)
    {
        _logger.LogInformation($"{nameof(TagRepository.DeleteAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(TagRepository.DeleteAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogInformation($"{nameof(TagRepository.Delete)}");

        var entityDb = _context.Tags.FirstOrDefault(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(TagRepository.Delete)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Tags.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(TagRepository.DeleteAsync)}");

        var entityDb = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(TagRepository.DeleteAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Tags.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Tag>? GetAll(bool DisableTracking = true)
    {
        IQueryable<Tag> query = _context.Tags
                .Include(t => t.Files)
                .Include(t => t.FileGroups);

        if (DisableTracking)
            query = query.AsNoTracking();

        return query.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<Tag>?> GetAllAsync(bool DisableTracking = true)
    {
        IQueryable<Tag> query = _context.Tags
                .Include(t => t.Files)
                .Include(t => t.FileGroups);

        if (DisableTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Tag? GetById(Guid id, bool DisableTracking = true)
    {
        IQueryable<Tag> query = _context.Tags
                .Where(c => c.Id == id)
                .Include(t => t.Files)
                .Include(t => t.FileGroups);

        if (DisableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<Tag?> GetByIdAsync(Guid id, bool DisableTracking = true)
    {
        IQueryable<Tag> query = _context.Tags
                .Where(c => c.Id == id)
                .Include(t => t.Files)
                .Include(t => t.FileGroups);

        if (DisableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Tag entity)
    {
        _logger.LogInformation($"{nameof(TagRepository.Update)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(TagRepository.Update)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = GetById(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(TagRepository.Update)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.Tags.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Tag entity)
    {
        _logger.LogInformation($"{nameof(TagRepository.UpdateAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(TagRepository.UpdateAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = await GetByIdAsync(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(TagRepository.UpdateAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }


        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.Tags.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<Tag?> GetByNameAsync(string name, bool DisableTracking = true)
    {
        IQueryable<Tag> query = _context.Tags
                .Where(c => c.Name == name)
                .Include(t => t.Files)
                .Include(t => t.FileGroups);

        if (DisableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public Tag? GetByName(string name, bool DisableTracking = true)
    {
        IQueryable<Tag> query = _context.Tags
                .Where(c => c.Name == name)
                .Include(t => t.Files)
                .Include(t => t.FileGroups);

        if (DisableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<Tag?> GetByHashAsync(string hash, bool DisableTracking = true) => null;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public Tag? GetByHash(string hash, bool DisableTracking = true) => null;

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private Tag UpdateCurrentEntity(Tag sourse, Tag recipient)
    {
        recipient.Files = sourse.Files;
        recipient.Name = sourse.Name;
        recipient.FileGroups = sourse.FileGroups;

        return recipient;
    }
}