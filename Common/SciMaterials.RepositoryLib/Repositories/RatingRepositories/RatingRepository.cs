using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Repositories.CategorysRepositories;
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
        _logger.LogTrace($"Логгер встроен в {nameof(RatingRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Rating entity)
    {
        _logger.LogInformation($"{nameof(RatingRepository.Add)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(RatingRepository.Add)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Ratings.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Rating entity)
    {
        _logger.LogInformation($"{nameof(RatingRepository.AddAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(RatingRepository.AddAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.Ratings.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(Rating entity)
    {
        _logger.LogInformation($"{nameof(RatingRepository.Delete)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(RatingRepository.Delete)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(Rating entity)
    {
        _logger.LogInformation($"{nameof(RatingRepository.DeleteAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(RatingRepository.DeleteAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogInformation($"{nameof(RatingRepository.Delete)}");

        var entityDb = _context.Ratings.FirstOrDefault(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(RatingRepository.Delete)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Ratings.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(RatingRepository.DeleteAsync)}");

        var entityDb = await _context.Ratings.FirstOrDefaultAsync(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(RatingRepository.DeleteAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Ratings.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Rating>? GetAll(bool DisableTracking = true)
    {
        IQueryable<Rating> query = _context.Ratings
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup);

        if (DisableTracking)
            query = query.AsNoTracking();

        return query.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<Rating>?> GetAllAsync(bool DisableTracking = true)
    {
        IQueryable<Rating> query = _context.Ratings
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup);

        if (DisableTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Rating? GetById(Guid id, bool DisableTracking = true)
    {
        IQueryable<Rating> query = _context.Ratings
                .Where(c => c.Id == id)
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup);

        if (DisableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<Rating?> GetByIdAsync(Guid id, bool DisableTracking = true)
    {
        IQueryable<Rating> query = _context.Ratings
                .Where(c => c.Id == id)
                .Include(r => r.File)
                .Include(r => r.User)
                .Include(r => r.FileGroup);

        if (DisableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Rating entity)
    {
        _logger.LogInformation($"{nameof(RatingRepository.Update)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(RatingRepository.Update)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = GetById(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(RatingRepository.Update)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.Ratings.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Rating entity)
    {
        _logger.LogInformation($"{nameof(RatingRepository.UpdateAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(RatingRepository.UpdateAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = await GetByIdAsync(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(RatingRepository.UpdateAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.Ratings.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<Rating?> GetByNameAsync(string name, bool DisableTracking = true) => null;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public Rating? GetByName(string name, bool DisableTracking = true) => null;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<Rating?> GetByHashAsync(string hash, bool DisableTracking = true) => null;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public Rating? GetByHash(string hash, bool DisableTracking = true) => null;

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