using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.Repositories.RatingRepositories;


/// <summary> Интерфейс репозитория для <see cref="Rating"/>. </summary>
public interface IRatingRepository : IRepository<Rating> { }

/// <summary> Репозиторий для <see cref="Rating"/>. </summary>
public class RatingRepository : IRatingRepository
{
    private readonly ILogger _logger;
    private readonly ISciMaterialsContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public RatingRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogDebug($"Логгер встроен в {nameof(RatingRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Rating entity)
    {
        _logger.LogDebug(nameof(Add));

        if (entity is null) return;
        _context.Ratings.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Rating entity)
    {
        _logger.LogDebug(nameof(AddAsync));

        if (entity is null) return;
        await _context.Ratings.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(Rating entity)
    {
        _logger.LogDebug(nameof(Delete));
        if (entity is null || entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(Rating entity)
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

        var RatingDb = _context.Ratings.FirstOrDefault(c => c.Id == id);
        if (RatingDb is null) return;
        _context.Ratings.Remove(RatingDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug(nameof(DeleteAsync));

        var RatingDb = await _context.Ratings.FirstOrDefaultAsync(c => c.Id == id);
        if (RatingDb is null) return;
        _context.Ratings.Remove(RatingDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Rating>? GetAll(bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetAll));

        if (DisableTracking)
            return _context.Ratings
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup)
                .AsNoTracking()
                .ToList();
        else
            return _context.Ratings
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup)
                .ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<Rating>?> GetAllAsync(bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetAllAsync));

        if (DisableTracking)
            return await _context.Ratings
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup)
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.Ratings
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup)
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Rating? GetById(Guid id, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetById));

        if (DisableTracking)
            return _context.Ratings
                .Where(c => c.Id == id)
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Ratings
                .Where(c => c.Id == id)
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<Rating?> GetByIdAsync(Guid id, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByIdAsync));

        if (DisableTracking)
            return (await _context.Ratings
                .Where(c => c.Id == id)
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Ratings
                .Where(c => c.Id == id)
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Rating entity)
    {
        _logger.LogDebug(nameof(Update));

        if (entity is null) return;
        var RatingDb = GetById(entity.Id, false);

        RatingDb = UpdateCurrentEntity(entity, RatingDb!);
        _context.Ratings.Update(RatingDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Rating entity)
    {
        _logger.LogDebug(nameof(UpdateAsync));

        if (entity is null) return;
        var RatingDb = await GetByIdAsync(entity.Id, false);

        RatingDb = UpdateCurrentEntity(entity, RatingDb!);
        _context.Ratings.Update(RatingDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<Rating?> GetByNameAsync(string name, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByNameAsync));

        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public Rating? GetByName(string name, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByName));

        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<Rating?> GetByHashAsync(string hash, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByHashAsync));
        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public Rating? GetByHash(string hash, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByHash));
        return null!;
    }

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private Rating UpdateCurrentEntity(Rating sourse, Rating recipient)
    {
        recipient.File = sourse.File;
        recipient.FileId = sourse.FileId;
        recipient.FileGroup = sourse.FileGroup;
        recipient.FileGroupId = sourse.FileGroupId;
        recipient.RatingValue = sourse.RatingValue;
        recipient.User = sourse.User;
        recipient.AuthorId = sourse.AuthorId;

        return recipient;
    }
}