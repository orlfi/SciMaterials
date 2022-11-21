using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Repositories.Files;
using SciMaterials.DAL.Repositories.Ratings;
using SciMaterials.DAL.Repositories.Users;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories;

using File = SciMaterials.DAL.Resources.Contracts.Entities.File;

namespace SciMaterials.DAL.UnitOfWork;

public class SciMaterialsFilesUnitOfWork : IUnitOfWork<SciMaterialsContext>
{
    private readonly ILogger<SciMaterialsFilesUnitOfWork> _logger;
    private readonly SciMaterialsContext _context;

    private bool disposed;
    private Dictionary<Type, object>? _repositories = new();

    /// <summary> ctor. </summary>
    /// <param name="logger"></param>
    /// <param name="context"></param>
    /// <exception cref="ArgumentException"></exception>
    public SciMaterialsFilesUnitOfWork(
        ILogger<SciMaterialsFilesUnitOfWork> logger,
        SciMaterialsContext context)
    {
        _logger = logger;
        _logger.LogDebug($"Логгер встроен в {nameof(SciMaterialsFilesUnitOfWork)}.");

        _context = context ?? throw new ArgumentException(nameof(context));

        Initialise();
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.GetRepository{TEntity}"/>
    public IRepository<T> GetRepository<T>() where T : class
    {
        _logger.LogDebug($"{nameof(SciMaterialsFilesUnitOfWork)} >>> {nameof(GetRepository)}.");

        if (_repositories == null)
            _repositories = new Dictionary<Type, object>();

        var type = typeof(T);

        return (IRepository<T>)_repositories[type];
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.SaveContext()"/>
    public int SaveContext()
    {
        _logger.LogInformation($"{nameof(SciMaterialsFilesUnitOfWork)} >>> {nameof(SaveContext)}.");
        try
        {
            return _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(SciMaterialsFilesUnitOfWork)} >>> {nameof(SaveContext)}. Ошибка при попытке сохранений изменений контекста. >>> {ex.Message}");
            return 0;
        }
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.SaveContextAsync()"/>
    public async Task<int> SaveContextAsync()
    {
        _logger.LogInformation($"{nameof(SciMaterialsFilesUnitOfWork)} >>> {nameof(SaveContextAsync)}.");
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(SciMaterialsFilesUnitOfWork)} >>> {nameof(SaveContextAsync)}. Ошибка при попытке сохранений изменений контекста. >>> {ex.Message} >>> {ex.InnerException?.Message ?? ""}");
            return 0;
        }
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.BeginTransaction(bool)"/>
    public IDbContextTransaction BeginTransaction(bool UseIfExists = false)
    {
        var transaction = _context.Database.CurrentTransaction;
        if (transaction == null)
        {
            return _context.Database.BeginTransaction();
        }

        return UseIfExists ? transaction : _context.Database.BeginTransaction();
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.BeginTransactionAsync(bool)"/>
    public Task<IDbContextTransaction> BeginTransactionAsync(bool UseIfExists = false)
    {
        throw new NotImplementedException();
    }

    private void Initialise()
    {
        _repositories!.Add(typeof(User), new AuthorRepository(_context, _logger));
        _repositories!.Add(typeof(File), new FileRepository(_context, _logger));
        _repositories!.Add(typeof(Category), new CategoryRepository(_context, _logger));
        _repositories!.Add(typeof(Comment), new CommentRepository(_context, _logger));
        _repositories!.Add(typeof(ContentType), new ContentTypeRepository(_context, _logger));
        _repositories!.Add(typeof(FileGroup), new FileGroupRepository(_context, _logger));
        _repositories!.Add(typeof(Rating), new RatingRepository(_context, _logger));
        _repositories!.Add(typeof(Tag), new TagRepository(_context, _logger));
        _repositories!.Add(typeof(Author), new AuthorRepository(_context, _logger));
        _repositories!.Add(typeof(Url), new UrlRepository(_context, _logger));
    }

    #region Dispose

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            disposed = true;
        }
    }

    #endregion
}