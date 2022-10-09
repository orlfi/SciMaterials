
using SciMaterials.DAL.Models;
using SciMaterials.RepositoryTests.Fixtures;

namespace SciMaterials.RepositoryTests.Tests;

public class CategoryRepositoryTests : IClassFixture<UnitOfWorkFixture>
{
    private readonly UnitOfWorkFixture _fixture;

    public CategoryRepositoryTests(UnitOfWorkFixture fixture)
    {
        _fixture = fixture;
    }

    #region GetAll

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetAll_AsNoTracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        var sut = _fixture.Create();

        //act
        var actual = sut.GetRepository<Category>().GetAll().Count;

        //assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetAll_Tracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        var sut = _fixture.Create();

        //act
        var actual = sut.GetRepository<Category>().GetAll(false).Count;

        //assert
        Assert.Equal(expected, actual);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        var sut = _fixture.Create();

        //act
        var categories = await sut.GetRepository<Category>().GetAllAsync();

        int actual = 0;
        if(categories is not null) 
            actual = categories.Count;

        //assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetAllAsync_Tracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        var sut = _fixture.Create();

        //act
        var categories = await sut.GetRepository<Category>().GetAllAsync(false);

        int actual = 0;
        if (categories is not null)
            actual = categories.Count;

        //assert
        Assert.Equal(expected, actual);
    }

    #endregion

}