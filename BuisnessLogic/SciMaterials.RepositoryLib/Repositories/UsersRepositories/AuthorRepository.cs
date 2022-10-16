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
        _logger.LogTrace($"Логгер встроен в {nameof(AuthorRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Author entity)
    {
        _logger.LogInformation($"{nameof(AuthorRepository.Add)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(AuthorRepository.Add)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Authors.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Author entity)
    {
        _logger.LogInformation($"{nameof(AuthorRepository.AddAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(AuthorRepository.AddAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.Authors.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(Author entity)
    {
        _logger.LogInformation($"{nameof(AuthorRepository.Delete)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(AuthorRepository.Delete)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(Author entity)
    {
        _logger.LogInformation($"{nameof(AuthorRepository.DeleteAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(AuthorRepository.DeleteAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogInformation($"{nameof(AuthorRepository.Delete)}");

        var entityDb = _context.Authors.FirstOrDefault(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(AuthorRepository.Delete)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Authors.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(AuthorRepository.DeleteAsync)}");

        var entityDb = await _context.Authors.FirstOrDefaultAsync(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(AuthorRepository.DeleteAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Authors.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Author>? GetAll(bool disableTracking = true, bool include = false)
    {
        IQueryable<Author> query = _context.Authors.Where(a => !a.IsDeleted);

        if (include)
            query.Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<Author>?> GetAllAsync(bool disableTracking = true, bool include = false)
    {
        IQueryable<Author> query = _context.Authors.Where(a => !a.IsDeleted);

        if (include)
            query.Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Author? GetById(Guid id, bool disableTracking = true, bool include = false)
    {
        IQueryable<Author> query = _context.Authors
                .Where(c => c.Id == id && !c.IsDeleted);

        if (include)
            query.Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<Author?> GetByIdAsync(Guid id, bool disableTracking = true, bool include = false)
    {
        IQueryable<Author> query = _context.Authors
                .Where(c => c.Id == id && !c.IsDeleted);

        if (include)
            query.Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Author entity)
    {
        _logger.LogInformation($"{nameof(AuthorRepository.Update)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(AuthorRepository.Update)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = GetById(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(AuthorRepository.Update)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.Authors.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Author entity)
    {
        _logger.LogInformation($"{nameof(AuthorRepository.UpdateAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(AuthorRepository.UpdateAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = await GetByIdAsync(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(AuthorRepository.UpdateAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.Authors.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<Author?> GetByNameAsync(string name, bool disableTracking = true, bool include = false)
    {
        IQueryable<Author> query = _context.Authors
                .Where(c => c.Name == name && !c.IsDeleted);
        
        if (include)
            query.Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public Author? GetByName(string name, bool disableTracking = true, bool include = false)
    {
        IQueryable<Author> query = _context.Authors
                .Where(c => c.Name == name && !c.IsDeleted);

        if (include)
            query.Include(u => u.Comments)
                .Include(u => u.Files)
                .Include(u => u.Ratings)
                .Include(u => u.User);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<Author?> GetByHashAsync(string hash, bool disableTracking = true, bool include = false) => null;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public Author? GetByHash(string hash, bool disableTracking = true, bool include = false) => null;

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