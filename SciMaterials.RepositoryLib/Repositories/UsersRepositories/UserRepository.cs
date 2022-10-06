using Microsoft.EntityFrameworkCore;
using NLog;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Repositories.FilesRepositories;

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
        _logger.Debug($"Логгер встроен в {nameof(UserRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(User entity)
    {
        _logger.Debug($"{nameof(UserRepository.Add)}");

        if (entity is null) return;
        _context.Users.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(User entity)
    {
        _logger.Debug($"{nameof(UserRepository.AddAsync)}");

        if (entity is null) return;
        await _context.Users.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.Debug($"{nameof(UserRepository.Delete)}");

        var UserDb = _context.Users.FirstOrDefault(c => c.Id == id);
        if (UserDb is null) return;
        _context.Users.Remove(UserDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.Debug($"{nameof(UserRepository.DeleteAsync)}");

        var UserDb = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
        if (UserDb is null) return;
        _context.Users.Remove(UserDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<User> GetAll(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(UserRepository.GetAll)}");

        if (disableTracking)
            return _context.Users
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .AsNoTracking()
                .ToList();
        else
            return _context.Users.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<User>> GetAllAsync(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(UserRepository.GetAllAsync)}");

        if (disableTracking)
            return await _context.Users
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.Users
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public User GetById(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(UserRepository.GetById)}");

        if (disableTracking)
            return _context.Users
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Users
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<User> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(UserRepository.GetByIdAsync)}");

        if (disableTracking)
            return (await _context.Users
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Users
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(User entity)
    {
        _logger.Debug($"{nameof(UserRepository.Update)}");

        if (entity is null) return;
        var UserDb = GetById(entity.Id, false);

        UserDb = UpdateCurrentEnity(entity, UserDb);
        _context.Users.Update(UserDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(User entity)
    {
        _logger.Debug($"{nameof(UserRepository.UpdateAsync)}");

        if (entity is null) return;
        var UserDb = await GetByIdAsync(entity.Id, false);

        UserDb = UpdateCurrentEnity(entity, UserDb);
        _context.Users.Update(UserDb);
    }

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private User UpdateCurrentEnity(User sourse, User recipient)
    {
        recipient.Files = sourse.Files;
        recipient.Name = sourse.Name;
        recipient.Comments = sourse.Comments;
        recipient.Email = sourse.Email;
        recipient.Ratings = sourse.Ratings;

        return recipient;
    }
}