
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.Data.Repositories;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.DAL.Repositories.FilesRepositories;

/// <summary> Интерфейс репозитория для <see cref="File"/>. </summary>
public interface IFileRepository : IRepository<File> { }

/// <summary> Репозиторий для <see cref="File"/>. </summary>
public class FileRepository : IFileRepository
{
    private readonly ILogger _logger;
    private readonly ISciMaterialsContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public FileRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogDebug($"Логгер встроен в {nameof(FileRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(File entity)
    {
        _logger.LogDebug($"{nameof(FileRepository.Add)}");

        if (entity is null) return;
        _context.Files.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(File entity)
    {
        _logger.LogDebug($"{nameof(FileRepository.AddAsync)}");

        if (entity is null) return;
        await _context.Files.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(File entity)
    {
        _logger.LogDebug($"{nameof(FileRepository.Delete)}");
        if (entity is null || entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(File entity)
    {
        _logger.LogDebug($"{nameof(FileRepository.DeleteAsync)}");
        if (entity is null || entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogDebug($"{nameof(FileRepository.Delete)}");

        var FileDb = _context.Files.FirstOrDefault(c => c.Id == id);
        if (FileDb is null) return;
        _context.Files.Remove(FileDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug($"{nameof(FileRepository.DeleteAsync)}");

        var FileDb = await _context.Files.FirstOrDefaultAsync(c => c.Id == id);
        if (FileDb is null) return;
        _context.Files.Remove(FileDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<File>? GetAll(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(FileRepository.GetAll)}");

        if (disableTracking)
            return _context.Files
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .AsNoTracking()
                .ToList();
        else
            return _context.Files
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<File>?> GetAllAsync(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(FileRepository.GetAllAsync)}");

        if (disableTracking)
            return await _context.Files
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.Files
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public File? GetById(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(FileRepository.GetById)}");

        if (disableTracking)
            return _context.Files
                .Where(c => c.Id == id)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Files
                .Where(c => c.Id == id)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<File?> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(FileRepository.GetByIdAsync)}");

        if (disableTracking)
            return (await _context.Files
                .Where(c => c.Id == id)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Files
                .Where(c => c.Id == id)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(File entity)
    {
        _logger.LogDebug($"{nameof(FileRepository.Update)}");

        if (entity is null) return;
        var FileDb = GetById(entity.Id, false);

        FileDb = UpdateCurrentEntity(entity, FileDb!);
        _context.Files.Update(FileDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(File entity)
    {
        _logger.LogDebug($"{nameof(FileRepository.UpdateAsync)}");

        if (entity is null) return;
        var FileDb = await GetByIdAsync(entity.Id, false);

        FileDb = UpdateCurrentEntity(entity, FileDb!);
        _context.Files.Update(FileDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<File?> GetByNameAsync(string name, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(FileRepository.GetByNameAsync)}");

        if (disableTracking)
            return (await _context.Files
                .Where(c => c.Name == name)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Files
                .Where(c => c.Name == name)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public File? GetByName(string name, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(FileRepository.GetByName)}");

        if (disableTracking)
            return _context.Files
                .Where(c => c.Name == name)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Files
                .Where(c => c.Name == name)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<File?> GetByHashAsync(string hash, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(FileRepository.GetByHashAsync)}");

        if (disableTracking)
            return (await _context.Files
                .Where(c => c.Hash == hash)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Files
                .Where(c => c.Hash == hash)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public File? GetByHash(string hash, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(FileRepository.GetByHash)}");

        if (disableTracking)
            return _context.Files
                .Where(c => c.Hash == hash)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Files
                .Where(c => c.Hash == hash)
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings)
                .FirstOrDefault()!;
    }

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private File UpdateCurrentEntity(File sourse, File recipient)
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

        recipient.Url = sourse.Url;
        recipient.Size = sourse.Size;
        recipient.Hash = sourse.Hash;
        recipient.ContentTypeId = sourse.ContentTypeId;
        recipient.FileGroupId = sourse.FileGroupId;
        recipient.ContentType = sourse.ContentType;
        recipient.FileGroup = sourse.FileGroup;

        return recipient;
    }
}