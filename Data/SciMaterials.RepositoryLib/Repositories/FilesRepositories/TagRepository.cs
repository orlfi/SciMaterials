
using Microsoft.EntityFrameworkCore;
using NLog;
using SciMaterials.DAL.Models;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.Repositories.FilesRepositories;

/// <summary> Интерфейс репозитория для <see cref="Tag"/>. </summary>
public interface ITagRepository : IRepository<Tag> { }

/// <summary> Репозиторий для <see cref="Tag"/>. </summary>
public class TagRepository : ITagRepository
{
    private readonly ILogger _logger;
    private readonly DbContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public TagRepository(
        DbContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Логгер встроен в {nameof(TagRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Tag entity)
    {
        _logger.Debug($"{nameof(TagRepository.Add)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Tag entity)
    {
        _logger.Debug($"{nameof(TagRepository.AddAsync)}");
    }

    public async Task DeleteAsync(Tag entity) { throw new NotImplementedException(); }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.Debug($"{nameof(TagRepository.Delete)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.Debug($"{nameof(TagRepository.DeleteAsync)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Tag> GetAll(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(TagRepository.GetAll)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public Task<List<Tag>> GetAllAsync(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(TagRepository.GetAllAsync)}");



        return null!;
    }

    public async Task<Tag?> GetByHashAsync(string hash, bool DisableTracking = true) { throw new NotImplementedException(); }
    public Tag? GetByHash(string hash, bool DisableTracking = true) { throw new NotImplementedException(); }
    public async Task<Tag?> GetByNameAsync(string name, bool DisableTracking = true) { throw new NotImplementedException(); }
    public Tag? GetByName(string name, bool DisableTracking = true) { throw new NotImplementedException(); }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Tag GetById(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(TagRepository.GetById)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public Task<Tag> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(TagRepository.GetByIdAsync)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Tag entity)
    {
        _logger.Debug($"{nameof(TagRepository.Update)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Tag entity)
    {
        _logger.Debug($"{nameof(TagRepository.UpdateAsync)}");
    }

    public void Delete(Tag entity) { throw new NotImplementedException(); }
}