
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Repositories.CategorysRepositories;
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
        _logger.LogDebug($"Логгер встроен в {nameof(TagRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Tag entity)
    {
        _logger.LogDebug($"{nameof(TagRepository.Add)}");

        if (entity is null) return;
        _context.Tags.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Tag entity)
    {
        _logger.LogDebug($"{nameof(TagRepository.AddAsync)}");

        if (entity is null) return;
        await _context.Tags.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(Tag entity)
    {
        _logger.LogDebug($"{nameof(TagRepository.Delete)}");
        if (entity is null || entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(Tag entity)
    {
        _logger.LogDebug($"{nameof(TagRepository.DeleteAsync)}");
        if (entity is null || entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogDebug($"{nameof(TagRepository.Delete)}");

        var TagDb = _context.Tags.FirstOrDefault(c => c.Id == id);
        if (TagDb is null) return;
        _context.Tags.Remove(TagDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug($"{nameof(TagRepository.DeleteAsync)}");

        var TagDb = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
        if (TagDb is null) return;
        _context.Tags.Remove(TagDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Tag>? GetAll(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(TagRepository.GetAll)}");

        if (disableTracking)
            return _context.Tags
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .AsNoTracking()
                .ToList();
        else
            return _context.Tags
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<Tag>?> GetAllAsync(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(TagRepository.GetAllAsync)}");

        if (disableTracking)
            return await _context.Tags
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.Tags
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Tag? GetById(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(TagRepository.GetById)}");

        if (disableTracking)
            return _context.Tags
                .Where(c => c.Id == id)
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Tags
                .Where(c => c.Id == id)
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<Tag?> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(TagRepository.GetByIdAsync)}");

        if (disableTracking)
            return (await _context.Tags
                .Where(c => c.Id == id)
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Tags
                .Where(c => c.Id == id)
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Tag entity)
    {
        _logger.LogDebug($"{nameof(TagRepository.Update)}");

        if (entity is null) return;
        var TagDb = GetById(entity.Id, false);

        TagDb = UpdateCurrentEntity(entity, TagDb!);
        _context.Tags.Update(TagDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Tag entity)
    {
        _logger.LogDebug($"{nameof(TagRepository.UpdateAsync)}");

        if (entity is null) return;
        var TagDb = await GetByIdAsync(entity.Id, false);

        TagDb = UpdateCurrentEntity(entity, TagDb!);
        _context.Tags.Update(TagDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<Tag?> GetByNameAsync(string name, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(TagRepository.GetByNameAsync)}");

        if (disableTracking)
            return (await _context.Tags
                .Where(c => c.Name == name)
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Tags
                .Where(c => c.Name == name)
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public Tag? GetByName(string name, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(TagRepository.GetByName)}");

        if (disableTracking)
            return _context.Tags
                .Where(c => c.Name == name)
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Tags
                .Where(c => c.Name == name)
                .Include(t => t.Files)
                .Include(t => t.FileGroups)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<Tag?> GetByHashAsync(string hash, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(TagRepository.GetByHashAsync)}");
        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public Tag? GetByHash(string hash, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(TagRepository.GetByHash)}");
        return null!;
    }

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