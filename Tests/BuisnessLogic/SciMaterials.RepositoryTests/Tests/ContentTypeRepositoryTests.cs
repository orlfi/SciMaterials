using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Contracts.Entities;
using SciMaterials.DAL.Contracts.Repositories.Files;
using SciMaterials.DAL.Repositories.Files;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Tests;

public class ContentTypeRepositoryTests
{
    private IContentTypeRepository _contentTypeRepository;
    private SciMaterialsContext _context;

    public ContentTypeRepositoryTests()
    {
        _context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<SciMaterialsFilesUnitOfWork>(loggerFactory);

        _contentTypeRepository = new ContentTypeRepository(_context, logger);
    }


    #region GetAll

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetAll_AsNoTracking_ItShould_contains_contentType_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedState = EntityState.Detached;

        //act
        var contentTypes = _contentTypeRepository.GetAll();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedState, _context.Entry(contentTypes![0]).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetAll_Tracking_ItShould_contains_contentType_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var contentTypes = _contentTypeRepository.GetAll(false);
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_contentType_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var contentTypes = await _contentTypeRepository.GetAllAsync();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetAllAsync_Tracking_ItShould_contains_contentType_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var contentTypes = await _contentTypeRepository.GetAllAsync(false);
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void AddAsync_ItShould_contains_contentType_increase_by_1()
    {
        //arrange
        var expected = (await _contentTypeRepository.GetAllAsync())!.Count + 1;
        var category = ContentTypeHelper.GetOne();

        //act
        await _contentTypeRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        var contentTypes = await _contentTypeRepository.GetAllAsync();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var categoryDb = await _contentTypeRepository.GetByIdAsync(category.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(category.Name, categoryDb.Name);
        Assert.Equal(category.FileExtension, categoryDb.FileExtension);
        Assert.Equal(category.Files.ToList()[0].Id, categoryDb.Files.ToList()[0].Id);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void Add_ItShould_contains_contentType_3()
    {
        //arrange
        var expected = _contentTypeRepository.GetAll()!.Count + 1;
        var category = ContentTypeHelper.GetOne();

        //act
        _contentTypeRepository.Add(category);
        _context.SaveChanges();

        var contentTypes = _contentTypeRepository.GetAll();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var categoryDb = _contentTypeRepository.GetById(category.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(category.Name, categoryDb.Name);
        Assert.Equal(category.FileExtension, categoryDb.FileExtension);
        Assert.Equal(category.Files.ToList()[0].Id, categoryDb.Files.ToList()[0].Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(category);
        await _context.SaveChangesAsync();
        var expected = (await _contentTypeRepository.GetAllAsync())!.Count - 1;

        //act
        await _contentTypeRepository.DeleteAsync(category.Id);
        await _context.SaveChangesAsync();

        var contentTypes = await _contentTypeRepository.GetAllAsync();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var removedContentType = await _contentTypeRepository.GetByIdAsync(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedContentType);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(category);
        _context.SaveChanges();
        var expected = _contentTypeRepository.GetAll()!.Count - 1;

        //act
        _contentTypeRepository.Delete(category.Id);
        _context.SaveChanges();

        var contentTypes = _contentTypeRepository.GetAll();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var removedContentType = _contentTypeRepository.GetById(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedContentType);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var category = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _contentTypeRepository.GetByIdAsync(category.Id, false);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var category = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _contentTypeRepository.GetByIdAsync(category.Id, true);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var category = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _contentTypeRepository.GetById(category.Id, false);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var category = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _contentTypeRepository.GetById(category.Id, true);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _contentTypeRepository.GetByIdAsync(category.Id);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _contentTypeRepository.GetById(category.Id);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
    }

    #endregion

    #region GetByName

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var category = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _contentTypeRepository.GetByNameAsync(category.Name, true);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByName_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var category = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _contentTypeRepository.GetByName(category.Name, false);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByNameAsync_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var category = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _contentTypeRepository.GetByNameAsync(category.Name, false);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByName_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var category = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _contentTypeRepository.GetByName(category.Name, true);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByNameAsync_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _contentTypeRepository.GetByNameAsync(category.Name);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByName_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _contentTypeRepository.GetByName(category.Name);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var category = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        category.Name = expectedName;
        await _contentTypeRepository.UpdateAsync(category);
        await _context.SaveChangesAsync();

        var categoryDb = await _contentTypeRepository.GetByIdAsync(category.Id);

        //assert
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(category.Name, expectedName);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void Update_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var category = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(category);
        _context.SaveChanges();

        //act
        category.Name = expectedName;
        _contentTypeRepository.Update(category);
        _context.SaveChanges();

        var categoryDb = _contentTypeRepository.GetById(category.Id);

        //assert
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(category.Name, expectedName);
    }

    #endregion

    #region GetByHash данные методы в репозитории не реализованы

    //[Fact]
    //[Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    //public async void GetByHashAsync_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var categoryDb = await _contentTypeRepository.GetByHashAsync(String.Empty);

    //    //assert
    //    Assert.Null(categoryDb);
    //}

    //[Fact]
    //[Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    //public void GetByHash_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var categoryDb = _contentTypeRepository.GetByHash(String.Empty);

    //    //assert
    //    Assert.Null(categoryDb);
    //}

    #endregion
}
