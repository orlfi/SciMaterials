
using Moq;
using NLog;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Repositories.CategorysRepositories;
using SciMaterials.DAL.UnitOfWork;

namespace SciMaterials.RepositoryTests.Helpers;

public static class UnitOfWorkHelper
{
    public static Mock<IUnitOfWork<SciMaterialsContext>> GetUnitOfWorkMock()
    {
        var context = new SciMateralsContextHelper().Context;
        var logger = LogManager.GetCurrentClassLogger();

        var unitOfWork = new Mock<IUnitOfWork<SciMaterialsContext>>();
        unitOfWork.Setup(x => x.GetRepository<Category>()).Returns(new CategoryRepository(context, logger));

        return unitOfWork;
    }
}