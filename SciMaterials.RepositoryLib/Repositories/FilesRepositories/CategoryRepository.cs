
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.Repositories.CategorysRepositories;


/// <summary> Интерфейс репозитория для <see cref="Category"/>. </summary>
public interface ICategoryRepository : IRepository<Category> { }

/// <summary> Репозиторий для <see cref="Category"/>. </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly ILogger _logger;
    private readonly ISciMaterialsContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public CategoryRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogDebug($"Логгер встроен в {nameof(CategoryRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Category entity)
    {
        _logger.LogDebug($"{nameof(CategoryRepository.Add)}");

        if (entity is null) return;
        _context.Categories.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Category entity)
    {
        _logger.LogDebug($"{nameof(CategoryRepository.AddAsync)}");

        if (entity is null) return;
        await _context.Categories.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogDebug($"{nameof(CategoryRepository.Delete)}");

        var categoryDb = _context.Categories.FirstOrDefault(c => c.Id == id);
        if (categoryDb is null) return;
        _context.Categories.Remove(categoryDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug($"{nameof(CategoryRepository.DeleteAsync)}");

        var categoryDb = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (categoryDb is null) return;
        _context.Categories.Remove(categoryDb!);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Category>? GetAll(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CategoryRepository.GetAll)}");

        if(disableTracking)
            return _context.Categories
                .Include(c => c.Files)
                .AsNoTracking()
                .ToList()!;
        else
            return _context.Categories
                .Include(c => c.Files)
                .ToList()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<Category>?> GetAllAsync(bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CategoryRepository.GetAllAsync)}");

        if (disableTracking)
            return await _context.Categories
                .Include(c => c.Files)
                .AsNoTracking()
                .ToListAsync()!;
        else
            return await _context.Categories
                .Include(c => c.Files)
                .ToListAsync()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Category GetById(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CategoryRepository.GetById)}");

        if (disableTracking)
            return _context.Categories
                .Where(c => c.Id == id)
                .Include(c => c.Files)
                .AsNoTracking()
                .FirstOrDefault()!;
        else
            return _context.Categories
                .Where(c => c.Id == id)
                .Include(c => c.Files)
                .FirstOrDefault()!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<Category?> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.LogDebug($"{nameof(CategoryRepository.GetByIdAsync)}");

        if (disableTracking)
            return (await _context.Categories
                .Where(c => c.Id == id)
                .Include(c => c.Files)
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        else
            return (await _context.Categories
                .Where(c => c.Id == id)
                .Include(c => c.Files)
                .FirstOrDefaultAsync())!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Category entity)
    {
        _logger.LogDebug($"{nameof(CategoryRepository.Update)}");

        if (entity is null) return;
        var categoryDb = GetById(entity.Id, false);

        categoryDb = UpdateCurrentEnity(entity, categoryDb);
        _context.Categories.Update(categoryDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Category entity)
    {
        _logger.LogDebug($"{nameof(CategoryRepository.UpdateAsync)}");

        if (entity is null) return;
        var categoryDb = await GetByIdAsync(entity.Id, false);

        categoryDb = UpdateCurrentEnity(entity, categoryDb!);
        _context.Categories.Update(categoryDb);
    }

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private Category UpdateCurrentEnity(Category sourse, Category recipient)
    {
        recipient.Description = sourse.Description;
        recipient.CreatedAt = sourse.CreatedAt;
        recipient.Files = sourse.Files;
        recipient.ParentId = sourse.ParentId;
        recipient.Name = sourse.Name;

        return recipient;
    }
}