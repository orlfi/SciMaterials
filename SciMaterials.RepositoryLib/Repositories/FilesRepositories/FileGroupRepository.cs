
using Microsoft.EntityFrameworkCore;
using NLog;
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
        _logger.Debug($"Логгер встроен в {nameof(FileGroupRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(FileGroup entity)
    {
        _logger.Debug($"{nameof(FileGroupRepository.Add)}");

        if (entity is null) return;
        _context.FileGroups.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(FileGroup entity)
    {
        _logger.Debug($"{nameof(FileGroupRepository.AddAsync)}");

        if (entity is null) return;
        await _context.FileGroups.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.Debug($"{nameof(FileGroupRepository.Delete)}");

        var FileGroupDb = _context.FileGroups.FirstOrDefault(c => c.Id == id);
        if (FileGroupDb is null) return;
        _context.FileGroups.Remove(FileGroupDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.Debug($"{nameof(FileGroupRepository.DeleteAsync)}");

        var FileGroupDb = await _context.FileGroups.FirstOrDefaultAsync(c => c.Id == id);
        if (FileGroupDb is null) return;
        _context.FileGroups.Remove(FileGroupDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<FileGroup> GetAll(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(FileGroupRepository.GetAll)}");

        if (disableTracking)
            return _context.FileGroups
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Category)
                .Include(fg => fg.Owner)
                .AsNoTracking()
                .ToList();
        else
            return _context.FileGroups
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Category)
                .Include(fg => fg.Owner)
                .ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<FileGroup>> GetAllAsync(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(FileGroupRepository.GetAllAsync)}");

        if (disableTracking)
            return await _context.FileGroups
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Category)
                .Include(fg => fg.Owner)
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.FileGroups
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Category)
                .Include(fg => fg.Owner)
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public FileGroup GetById(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(FileGroupRepository.GetById)}");

        if (disableTracking)
            return _context.FileGroups
                .Where(c => c.Id == id)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Category)
                .Include(fg => fg.Owner)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.FileGroups
                .Where(c => c.Id == id)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Category)
                .Include(fg => fg.Owner)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<FileGroup> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(FileGroupRepository.GetByIdAsync)}");

        if (disableTracking)
            return (await _context.FileGroups
                .Where(c => c.Id == id)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Category)
                .Include(fg => fg.Owner)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.FileGroups
                .Where(c => c.Id == id)
                .Include(fg => fg.Files)
                .Include(fg => fg.Tags)
                .Include(fg => fg.Ratings)
                .Include(fg => fg.Comments)
                .Include(fg => fg.Category)
                .Include(fg => fg.Owner)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(FileGroup entity)
    {
        _logger.Debug($"{nameof(FileGroupRepository.Update)}");

        if (entity is null) return;
        var FileGroupDb = GetById(entity.Id, false);

        FileGroupDb = UpdateCurrentEnity(entity, FileGroupDb);
        _context.FileGroups.Update(FileGroupDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(FileGroup entity)
    {
        _logger.Debug($"{nameof(FileGroupRepository.UpdateAsync)}");

        if (entity is null) return;
        var FileGroupDb = await GetByIdAsync(entity.Id, false);

        FileGroupDb = UpdateCurrentEnity(entity, FileGroupDb);
        _context.FileGroups.Update(FileGroupDb);
    }

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private FileGroup UpdateCurrentEnity(FileGroup sourse, FileGroup recipient)
    {
        recipient.Description = sourse.Description;
        recipient.CreatedAt = sourse.CreatedAt;
        recipient.Files = sourse.Files;
        recipient.Name = sourse.Name;
        recipient.Owner = sourse.Owner;
        recipient.OwnerId = sourse.OwnerId;
        recipient.Category = sourse.Category;
        recipient.CategoryId = sourse.CategoryId;
        recipient.Tags = sourse.Tags;
        recipient.Ratings = sourse.Ratings;
        recipient.Comments = sourse.Comments;
        recipient.Title = sourse.Title;

        return recipient;
    }
}