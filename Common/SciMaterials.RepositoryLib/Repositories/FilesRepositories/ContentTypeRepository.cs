using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.Repositories.ContentTypesRepositories;

/// <summary> Интерфейс репозитория для <see cref="ContentType"/>. </summary>
public interface IContentTypeRepository : IRepository<ContentType> { }

/// <summary> Репозиторий для <see cref="ContentType"/>. </summary>
public class ContentTypeRepository : IContentTypeRepository
{
    private readonly ILogger _logger;
    private readonly ISciMaterialsContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public ContentTypeRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogTrace($"Логгер встроен в {nameof(ContentTypeRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(ContentType entity)
    {
        _logger.LogInformation($"{nameof(ContentTypeRepository.Add)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(ContentTypeRepository.Add)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.ContentTypes.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(ContentType entity)
    {
        _logger.LogInformation($"{nameof(ContentTypeRepository.AddAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(ContentTypeRepository.AddAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.ContentTypes.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(ContentType entity)
    {
        _logger.LogInformation($"{nameof(ContentTypeRepository.Delete)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(ContentTypeRepository.Delete)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(ContentType entity)
    {
        _logger.LogInformation($"{nameof(ContentTypeRepository.DeleteAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(ContentTypeRepository.DeleteAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogInformation($"{nameof(ContentTypeRepository.Delete)}");

        var ContentTypeDb = _context.ContentTypes.FirstOrDefault(c => c.Id == id);

        if (ContentTypeDb is null)
        {
            _logger.LogError($"{nameof(ContentTypeRepository.Delete)} >>> argumentNullException {nameof(ContentTypeDb)}");
            throw new ArgumentNullException(nameof(ContentTypeDb));
        }

        _context.ContentTypes.Remove(ContentTypeDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(ContentTypeRepository.DeleteAsync)}");

        var ContentTypeDb = await _context.ContentTypes.FirstOrDefaultAsync(c => c.Id == id);

        if (ContentTypeDb is null)
        {
            _logger.LogError($"{nameof(ContentTypeRepository.DeleteAsync)} >>> argumentNullException {nameof(ContentTypeDb)}");
            throw new ArgumentNullException(nameof(ContentTypeDb));
        }

        _context.ContentTypes.Remove(ContentTypeDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<ContentType>? GetAll(bool DisableTracking = true)
    {
        IQueryable<ContentType> query = _context.ContentTypes
                .Include(ct => ct.Files);

        if (DisableTracking)
            query = query.AsNoTracking();

        return query.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<ContentType>?> GetAllAsync(bool DisableTracking = true)
    {
        IQueryable<ContentType> query = _context.ContentTypes
                .Include(ct => ct.Files);

        if (DisableTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public ContentType? GetById(Guid id, bool DisableTracking = true)
    {
        IQueryable<ContentType> query = _context.ContentTypes
                .Where(c => c.Id == id)
                .Include(ct => ct.Files);

        if (DisableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<ContentType?> GetByIdAsync(Guid id, bool DisableTracking = true)
    {
        IQueryable<ContentType> query = _context.ContentTypes
                .Where(c => c.Id == id)
                .Include(ct => ct.Files);

        if (DisableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(ContentType entity)
    {
        _logger.LogInformation($"{nameof(ContentTypeRepository.Update)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(ContentTypeRepository.Update)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = GetById(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(ContentTypeRepository.Update)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.ContentTypes.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(ContentType entity)
    {
        _logger.LogInformation($"{nameof(ContentTypeRepository.UpdateAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(ContentTypeRepository.UpdateAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = await GetByIdAsync(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(ContentTypeRepository.UpdateAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.ContentTypes.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<ContentType?> GetByNameAsync(string name, bool DisableTracking = true)
    {
        IQueryable<ContentType> query = _context.ContentTypes
                .Where(c => c.Name == name);

        if (DisableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public ContentType? GetByName(string name, bool DisableTracking = true)
    {
        IQueryable<ContentType> query = _context.ContentTypes
                .Where(c => c.Name == name);

        if (DisableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<ContentType?> GetByHashAsync(string hash, bool DisableTracking = true) => null;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public ContentType? GetByHash(string hash, bool DisableTracking = true) => null;

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private ContentType UpdateCurrentEntity(ContentType sourse, ContentType recipient)
    {
        recipient.Files = sourse.Files;
        recipient.Name = sourse.Name;
        recipient.FileExtension = sourse.FileExtension;

        return recipient;
    }
}