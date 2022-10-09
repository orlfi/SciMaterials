
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.Repositories.AuthorRepositories;
using SciMaterials.Data.UnitOfWork;
using SciMaterials.RepositoryTests.Helpers;

namespace SciMaterials.RepositoryTests.Tests;

public class AuthorRepositoryTests
{
    private IAuthorRepository _authorRepository;
    private SciMaterialsContext _context;

    public AuthorRepositoryTests()
    {
        _context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        _authorRepository = new AuthorRepository(_context, logger);
    }

    #region GetAll

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetAll_AsNoTracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var authors = _authorRepository.GetAll();
        int count = 0;
        if (authors is not null)
            count = authors.Count();

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(authors![0]).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetAll_Tracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var authors = _authorRepository.GetAll(false);
        int count = 0;
        if (authors is not null)
            count = authors.Count();

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(authors![0]).State);
    }

    #endregion
}