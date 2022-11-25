using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models.Base;

namespace SciMaterials.RepositoryLib.Repositories.FilesRepositories;

/// <summary> Интерфейс репозитория для <see cref="Resource"/>. </summary>
public interface IResourceRepository : IRepository<Resource> { }

/// <summary> Репозиторий для <see cref="Resource"/>. </summary>
public class ResourceRepository : IResourceRepository
{
    private readonly ISciMaterialsContext _context;
    private readonly ILogger _logger;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public ResourceRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogTrace($"Логгер встроен в {nameof(ResourceRepository)}");
        _context = context;
    }
    public List<Resource>? GetAll(bool disableTracking = true, bool include = false)
    {
        var query = _context.Set<Resource>()
            .Where(f => !f.IsDeleted);

        if (include)
            query = query
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.ToList();
    }

    public async Task<List<Resource>?> GetAllAsync(bool disableTracking = true, bool include = false)
    {
        var query = _context.Set<Resource>()
            .Where(f => !f.IsDeleted);

        if (include)
            query = query
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    public Resource? GetById(Guid id, bool disableTracking = true, bool include = false)
    {
        throw new NotImplementedException();
    }

    public Task<Resource?> GetByIdAsync(Guid id, bool disableTracking = true, bool include = false)
    {
        throw new NotImplementedException();
    }

    public Resource? GetByName(string name, bool disableTracking = true, bool include = false)
    {
        throw new NotImplementedException();
    }

    public Task<Resource?> GetByNameAsync(string name, bool disableTracking = true, bool include = false)
    {
        throw new NotImplementedException();
    }

    public int GetCount()
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetCountAsync()
    {
        var result = await _context.Set<Resource>().CountAsync();
        return result;
    }

    public List<Resource>? GetPage(int pageNumber, int pageSize, bool disableTracking = true, bool include = false)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Resource>?> GetPageAsync(int pageNumb, int pageSize, bool disableTracking = true, bool include = false)
    {
        IQueryable<Resource> query = _context.Set<Resource>()
            .Where(f => !f.IsDeleted);

        if (include)
            query = query
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query
            .Skip((pageNumb - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }


    public void Add(Resource entity)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Resource entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Resource entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Resource entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Resource? GetByHash(string hash, bool disableTracking = true, bool include = false)
    {
        throw new NotImplementedException();
    }

    public Task<Resource?> GetByHashAsync(string hash, bool disableTracking = true, bool include = false)
    {
        throw new NotImplementedException();
    }

    public void Update(Resource entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Resource entity)
    {
        throw new NotImplementedException();
    }
}