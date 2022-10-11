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
        _logger.LogDebug($"Логгер встроен в {nameof(FileGroupRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(FileGroup entity)
    {
        _logger.LogDebug(nameof(Add));

        if (entity is null) return;
        _context.FileGroups.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(FileGroup entity)
    {
        _logger.LogDebug(nameof(AddAsync));

        if (entity is null) return;
        await _context.FileGroups.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(FileGroup entity)
    {
        _logger.LogDebug(nameof(Delete));
        if (entity is null || entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(FileGroup entity)
    {
        _logger.LogDebug(nameof(DeleteAsync));
        if (entity is null || entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogDebug(nameof(Delete));

        var FileGroupDb = _context.FileGroups.FirstOrDefault(c => c.Id == id);
        if (FileGroupDb is null) return;
        _context.FileGroups.Remove(FileGroupDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug(nameof(DeleteAsync));

        var FileGroupDb = await _context.FileGroups.FirstOrDefaultAsync(c => c.Id == id);
        if (FileGroupDb is null) return;
        _context.FileGroups.Remove(FileGroupDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<FileGroup>? GetAll(bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetAll));

        if (DisableTracking)
            return _context.FileGroups
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .AsNoTracking()
                .ToList();
        else
            return _context.FileGroups
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<FileGroup>?> GetAllAsync(bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetAllAsync));

        if (DisableTracking)
            return await _context.FileGroups
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.FileGroups
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public FileGroup? GetById(Guid id, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetById));

        if (DisableTracking)
            return _context.FileGroups
                .Where(c => c.Id == id)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.FileGroups
                .Where(c => c.Id == id)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<FileGroup?> GetByIdAsync(Guid id, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByIdAsync));

        if (DisableTracking)
            return (await _context.FileGroups
                .Where(c => c.Id == id)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.FileGroups
                .Where(c => c.Id == id)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(FileGroup entity)
    {
        _logger.LogDebug(nameof(Update));

        if (entity is null) return;
        var FileGroupDb = GetById(entity.Id, false);

        FileGroupDb = UpdateCurrentEntity(entity, FileGroupDb!);
        _context.FileGroups.Update(FileGroupDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(FileGroup entity)
    {
        _logger.LogDebug(nameof(UpdateAsync));

        if (entity is null) return;
        var FileGroupDb = await GetByIdAsync(entity.Id, false);

        FileGroupDb = UpdateCurrentEntity(entity, FileGroupDb!);
        _context.FileGroups.Update(FileGroupDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<FileGroup?> GetByNameAsync(string name, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByNameAsync));

        if (DisableTracking)
            return (await _context.FileGroups
                .Where(c => c.Name == name)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.FileGroups
                .Where(c => c.Name == name)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public FileGroup? GetByName(string name, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByName));

        if (DisableTracking)
            return _context.FileGroups
                .Where(c => c.Name == name)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.FileGroups
                .Where(c => c.Name == name)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Categories)
                .Include(fg => fg.Author)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<FileGroup?> GetByHashAsync(string hash, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByHashAsync));
        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public FileGroup? GetByHash(string hash, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByHash));
        return null!;
    }

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