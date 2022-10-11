using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;

namespace SciMaterials.Data.Repositories.UserRepositories;

/// <summary> Интерфейс репозитория для <see cref="User"/>. </summary>
public interface IUserRepository : IRepository<User> { }

/// <summary> Репозиторий для <see cref="User"/>. </summary>
public class UserRepository : IUserRepository
{
    private readonly ILogger _logger;
    private readonly ISciMaterialsContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public UserRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogDebug($"Логгер встроен в {nameof(UserRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(User entity)
    {
        _logger.LogDebug($"{nameof(UserRepository.Add)}");

        if (entity is null) return;
        _context.Users.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(User entity)
    {
        _logger.LogDebug($"{nameof(UserRepository.AddAsync)}");

        if (entity is null) return;
        await _context.Users.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(User entity)
    {
        _logger.LogDebug($"{nameof(UserRepository.Delete)}");
        if (entity is null || entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(User entity)
    {
        _logger.LogDebug($"{nameof(UserRepository.DeleteAsync)}");
        if (entity is null || entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogDebug($"{nameof(UserRepository.Delete)}");

        var UserDb = _context.Users.FirstOrDefault(c => c.Id == id);
        if (UserDb is null) return;
        _context.Users.Remove(UserDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug($"{nameof(UserRepository.DeleteAsync)}");

        var UserDb = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
        if (UserDb is null) return;
        _context.Users.Remove(UserDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<User>? GetAll(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(UserRepository.GetAll)}");

        if (disableTracking)
            return _context.Users
                .AsNoTracking()
                .ToList();
        else
            return _context.Users.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<User>?> GetAllAsync(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(UserRepository.GetAllAsync)}");

        if (disableTracking)
            return await _context.Users
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.Users
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public User? GetById(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(UserRepository.GetById)}");

        if (disableTracking)
            return _context.Users
                .Where(c => c.Id == id)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Users
                .Where(c => c.Id == id)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<User?> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(UserRepository.GetByIdAsync)}");

        if (disableTracking)
            return (await _context.Users
                .Where(c => c.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Users
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(User entity)
    {
        _logger.LogDebug($"{nameof(UserRepository.Update)}");

        if (entity is null) return;
        var UserDb = GetById(entity.Id, false);

        UserDb = UpdateCurrentEntity(entity, UserDb!);
        _context.Users.Update(UserDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(User entity)
    {
        _logger.LogDebug($"{nameof(UserRepository.UpdateAsync)}");

        if (entity is null) return;
        var UserDb = await GetByIdAsync(entity.Id, false);

        UserDb = UpdateCurrentEntity(entity, UserDb!);
        _context.Users.Update(UserDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<User?> GetByNameAsync(string name, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(UserRepository.GetByNameAsync)}");

        if (disableTracking)
            return (await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Users
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public User? GetByName(string name, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(UserRepository.GetByName)}");

        if (disableTracking)
            return _context.Users
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Users
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<User?> GetByHashAsync(string hash, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(UserRepository.GetByHashAsync)}");
        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public User? GetByHash(string hash, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(UserRepository.GetByHash)}");
        return null!;
    }

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private User UpdateCurrentEntity(User sourse, User recipient)
    {
        return recipient;
    }
}