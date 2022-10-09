
using Moq;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Repositories.CategorysRepositories;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace SciMaterials.RepositoryTests.Helpers;

public static class UnitOfWorkHelper
{
    public static Mock<IUnitOfWork<SciMaterialsContext>> GetUnitOfWorkMock()
    {
        var context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = (ILoggerFactory)new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        var unitOfWork = new Mock<IUnitOfWork<SciMaterialsContext>>();
        unitOfWork.Setup(x => x.GetRepository<Category>()).Returns(new CategoryRepository(context, logger));

        return unitOfWork;
    }
}