
using Microsoft.EntityFrameworkCore;
using NLog;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Repositories.FilesRepositories;
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
        _logger.Debug($"Логгер встроен в {nameof(RatingRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Rating entity)
    {
        _logger.Debug($"{nameof(RatingRepository.Add)}");

        if (entity is null) return;
        _context.Ratings.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Rating entity)
    {
        _logger.Debug($"{nameof(RatingRepository.AddAsync)}");

        if (entity is null) return;
        await _context.Ratings.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.Debug($"{nameof(RatingRepository.Delete)}");

        //ToDo: уточнить по какому id получать экземпляр (заглушил c.FeildId)
        var RatingDb = _context.Ratings.FirstOrDefault(c => c.FileId == id);
        if (RatingDb is null) return;
        _context.Ratings.Remove(RatingDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.Debug($"{nameof(RatingRepository.DeleteAsync)}");

        //ToDo: уточнить по какому id получать экземпляр (заглушил c.FeildId)
        var RatingDb = await _context.Ratings.FirstOrDefaultAsync(c => c.FileId == id);
        if (RatingDb is null) return;
        _context.Ratings.Remove(RatingDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Rating> GetAll(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(RatingRepository.GetAll)}");

        if (disableTracking)
            return _context.Ratings
                .Include(r => r.File)
                .Include(r => r.User)
                .AsNoTracking()
                .ToList();
        else
            return _context.Ratings
                .Include(r => r.File)
                .Include(r => r.User)
                .ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<Rating>> GetAllAsync(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(RatingRepository.GetAllAsync)}");

        if (disableTracking)
            return await _context.Ratings
                .Include(r => r.File)
                .Include(r => r.User)
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.Ratings
                .Include(r => r.File)
                .Include(r => r.User)
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Rating GetById(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(RatingRepository.GetById)}");

        //ToDo: уточнить по какому id получать экземпляр (заглушил c.FeildId)
        if (disableTracking)
            return _context.Ratings
                .Where(c => c.FileId == id)
                .Include(r => r.File)
                .Include(r => r.User)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Ratings
                .Where(c => c.FileId == id)
                .Include(r => r.File)
                .Include(r => r.User)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<Rating> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(RatingRepository.GetByIdAsync)}");

        //ToDo: уточнить по какому id получать экземпляр (заглушил c.FeildId)
        if (disableTracking)
            return (await _context.Ratings
                .Where(c => c.FileId == id)
                .Include(r => r.File)
                .Include(r => r.User)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Ratings
                .Where(c => c.FileId == id)
                .Include(r => r.File)
                .Include(r => r.User)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Rating entity)
    {
        _logger.Debug($"{nameof(RatingRepository.Update)}");

        //ToDo: уточнить по какому id получать экземпляр (заглушил c.FeildId)
        if (entity is null) return;
        var RatingDb = GetById(entity.FileId, false);

        RatingDb = UpdateCurrentEnity(entity, RatingDb);
        _context.Ratings.Update(RatingDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Rating entity)
    {
        _logger.Debug($"{nameof(RatingRepository.UpdateAsync)}");

        //ToDo: уточнить по какому id получать экземпляр (заглушил c.FeildId)
        if (entity is null) return;
        var RatingDb = await GetByIdAsync(entity.FileId, false);

        RatingDb = UpdateCurrentEnity(entity, RatingDb);
        _context.Ratings.Update(RatingDb);
    }

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private Rating UpdateCurrentEnity(Rating sourse, Rating recipient)
    {
        recipient.File = sourse.File;
        recipient.FileId = sourse.FileId;
        recipient.RatingValue = sourse.RatingValue;
        recipient.User = sourse.User;
        recipient.UserId = sourse.UserId;

        return recipient;
    }
}