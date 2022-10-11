using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;

namespace SciMaterials.Data.Repositories.AuthorRepositories;

/// <summary> Интерфейс репозитория для <see cref="Author"/>. </summary>
public interface IAuthorRepository : IRepository<Author> { }

/// <summary> Репозиторий для <see cref="Author"/>. </summary>
public class AuthorRepository : IAuthorRepository
{
    private readonly ILogger _logger;
    private readonly ISciMaterialsContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public AuthorRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogDebug($"Логгер встроен в {nameof(AuthorRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Author entity)
    {
        _logger.LogDebug(nameof(Add));

        if (entity is null) return;
        _context.Authors.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Author entity)
    {
        _logger.LogDebug(nameof(AddAsync));

        if (entity is null) return;
        await _context.Authors.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(Author entity)
    {
        _logger.LogDebug(nameof(Delete));
        if (entity is null || entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(Author entity)
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

        var AuthorDb = _context.Authors.FirstOrDefault(c => c.Id == id);
        if (AuthorDb is null) return;
        _context.Authors.Remove(AuthorDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug(nameof(DeleteAsync));

        var AuthorDb = await _context.Authors.FirstOrDefaultAsync(c => c.Id == id);
        if (AuthorDb is null) return;
        _context.Authors.Remove(AuthorDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Author>? GetAll(bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetAll));

        if (DisableTracking)
            return _context.Authors
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User)
                .AsNoTracking()
                .ToList();
        else
            return _context.Authors.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<Author>?> GetAllAsync(bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetAllAsync));

        if (DisableTracking)
            return await _context.Authors
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User)
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.Authors
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User)
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Author? GetById(Guid id, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetById));

        if (DisableTracking)
            return _context.Authors
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Authors
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<Author?> GetByIdAsync(Guid id, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByIdAsync));

        if (DisableTracking)
            return (await _context.Authors
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Authors
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Author entity)
    {
        _logger.LogDebug(nameof(Update));

        if (entity is null) return;
        var AuthorDb = GetById(entity.Id, false);

        AuthorDb = UpdateCurrentEntity(entity, AuthorDb!);
        _context.Authors.Update(AuthorDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Author entity)
    {
        _logger.LogDebug(nameof(UpdateAsync));

        if (entity is null) return;
        var AuthorDb = await GetByIdAsync(entity.Id, false);

        AuthorDb = UpdateCurrentEntity(entity, AuthorDb!);
        _context.Authors.Update(AuthorDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<Author?> GetByNameAsync(string name, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByNameAsync));

        if (DisableTracking)
            return (await _context.Authors
                .Where(c => c.Name == name)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Authors
                .Where(c => c.Name == name)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public Author? GetByName(string name, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByName));

        if (DisableTracking)
            return _context.Authors
                .Where(c => c.Name == name)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Authors
                .Where(c => c.Name == name)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<Author?> GetByHashAsync(string hash, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByHashAsync));
        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public Author? GetByHash(string hash, bool DisableTracking = true)
    {
        _logger.LogDebug(nameof(GetByHash));
        return null!;
    }

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private Author UpdateCurrentEntity(Author sourse, Author recipient)
    {
        recipient.Files = sourse.Files;
        recipient.Name = sourse.Name;
        recipient.Surname = sourse.Surname;
        recipient.Comments = sourse.Comments;
        recipient.Phone = sourse.Phone;
        recipient.Email = sourse.Email;
        recipient.Ratings = sourse.Ratings;
        recipient.User = sourse.User;
        recipient.UserId = sourse.UserId;

        return recipient;
    }
}