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
        _logger.LogDebug($"{nameof(AuthorRepository.Add)}");

        if (entity is null) return;
        _context.Authors.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Author entity)
    {
        _logger.LogDebug($"{nameof(AuthorRepository.AddAsync)}");

        if (entity is null) return;
        await _context.Authors.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogDebug($"{nameof(AuthorRepository.Delete)}");

        var AuthorDb = _context.Authors.FirstOrDefault(c => c.Id == id);
        if (AuthorDb is null) return;
        _context.Authors.Remove(AuthorDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug($"{nameof(AuthorRepository.DeleteAsync)}");

        var AuthorDb = await _context.Authors.FirstOrDefaultAsync(c => c.Id == id);
        if (AuthorDb is null) return;
        _context.Authors.Remove(AuthorDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Author>? GetAll(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(AuthorRepository.GetAll)}");

        if (disableTracking)
            return _context.Authors
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .AsNoTracking()
                .ToList();
        else
            return _context.Authors.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<Author>?> GetAllAsync(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(AuthorRepository.GetAllAsync)}");

        if (disableTracking)
            return await _context.Authors
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .AsNoTracking()
                .ToListAsync();
        else
            return await _context.Authors
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Author? GetById(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(AuthorRepository.GetById)}");

        if (disableTracking)
            return _context.Authors
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Authors
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<Author?> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(AuthorRepository.GetByIdAsync)}");

        if (disableTracking)
            return (await _context.Authors
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Authors
                .Where(c => c.Id == id)
                .Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Author entity)
    {
        _logger.LogDebug($"{nameof(AuthorRepository.Update)}");

        if (entity is null) return;
        var AuthorDb = GetById(entity.Id, false);

        AuthorDb = UpdateCurrentEnity(entity, AuthorDb!);
        _context.Authors.Update(AuthorDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Author entity)
    {
        _logger.LogDebug($"{nameof(AuthorRepository.UpdateAsync)}");

        if (entity is null) return;
        var AuthorDb = await GetByIdAsync(entity.Id, false);

        AuthorDb = UpdateCurrentEnity(entity, AuthorDb!);
        _context.Authors.Update(AuthorDb);
    }

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private Author UpdateCurrentEnity(Author sourse, Author recipient)
    {
        recipient.Files = sourse.Files;
        recipient.Name = sourse.Name;
        recipient.Comments = sourse.Comments;
        recipient.Email = sourse.Email;
        recipient.Ratings = sourse.Ratings;

        return recipient;
    }
}