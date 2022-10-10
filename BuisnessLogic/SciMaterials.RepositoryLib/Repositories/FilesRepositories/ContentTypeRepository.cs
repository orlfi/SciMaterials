
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Repositories.CommentsRepositories;
using SciMaterials.DAL.Repositories.FilesRepositories;
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
        _logger.LogDebug($"Логгер встроен в {nameof(ContentTypeRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(ContentType entity)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.Add)}");

        if (entity is null) return;
        _context.ContentTypes.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(ContentType entity)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.AddAsync)}");

        if (entity is null) return;
        await _context.ContentTypes.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(ContentType entity)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.Delete)}");
        if (entity is null || entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(ContentType entity)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.DeleteAsync)}");
        if (entity is null || entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.Delete)}");

        var ContentTypeDb = _context.ContentTypes.FirstOrDefault(c => c.Id == id);
        if (ContentTypeDb is null) return;
        _context.ContentTypes.Remove(ContentTypeDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.DeleteAsync)}");

        var ContentTypeDb = await _context.ContentTypes.FirstOrDefaultAsync(c => c.Id == id);
        if (ContentTypeDb is null) return;
        _context.ContentTypes.Remove(ContentTypeDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<ContentType>? GetAll(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.GetAll)}");

        if (disableTracking)
            return _context.ContentTypes
                .Include(ct => ct.Files)
                .AsNoTracking()
                .ToList();
        else
            return _context.ContentTypes
                .Include(ct => ct.Files)
                .ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<ContentType>?> GetAllAsync(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.GetAllAsync)}");

        if (disableTracking)
            return await _context.ContentTypes
                .Include(ct => ct.Files)
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.ContentTypes
                .Include(ct => ct.Files)
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public ContentType? GetById(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.GetById)}");

        if (disableTracking)
            return _context.ContentTypes
                .Where(c => c.Id == id)
                .Include(ct => ct.Files)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.ContentTypes
                .Where(c => c.Id == id)
                .Include(ct => ct.Files)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<ContentType?> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.GetByIdAsync)}");

        if (disableTracking)
            return (await _context.ContentTypes
                .Where(c => c.Id == id)
                .Include(ct => ct.Files)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.ContentTypes
                .Where(c => c.Id == id)
                .Include(ct => ct.Files)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(ContentType entity)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.Update)}");

        if (entity is null) return;
        var ContentTypeDb = GetById(entity.Id, false);

        ContentTypeDb = UpdateCurrentEntity(entity, ContentTypeDb!);
        _context.ContentTypes.Update(ContentTypeDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(ContentType entity)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.UpdateAsync)}");

        if (entity is null) return;
        var ContentTypeDb = await GetByIdAsync(entity.Id, false);

        ContentTypeDb = UpdateCurrentEntity(entity, ContentTypeDb!);
        _context.ContentTypes.Update(ContentTypeDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<ContentType?> GetByNameAsync(string name, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.GetByNameAsync)}");
        if (disableTracking)
            return (await _context.ContentTypes
                .Where(c => c.Name == name)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.ContentTypes
                .Where(c => c.Name == name)
                .Include(ct => ct.Files)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public ContentType? GetByName(string name, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.GetByName)}");

        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<ContentType?> GetByHashAsync(string hash, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.GetByHashAsync)}");
        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public ContentType? GetByHash(string hash, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(ContentTypeRepository.GetByHash)}");
        return null!;
    }

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