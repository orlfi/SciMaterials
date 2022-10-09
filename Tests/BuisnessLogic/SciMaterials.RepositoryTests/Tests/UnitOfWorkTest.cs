
using SciMaterials.RepositoryTests.Fixtures;

namespace SciMaterials.RepositoryTests.Tests;

public class UnitOfWorkTest : IClassFixture<UnitOfWorkFixture>
{
    private readonly UnitOfWorkFixture _fixture;

    public UnitOfWorkTest(UnitOfWorkFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact]
    [Trait("UnitOfWorkTest", "UnderResting")]
    public void ItShould_UnitOfWork_instance_created()
    {
        //arrange
        var sut = _fixture.Create();

        //act

        //assert
        Assert.NotNull(sut);
    }
}