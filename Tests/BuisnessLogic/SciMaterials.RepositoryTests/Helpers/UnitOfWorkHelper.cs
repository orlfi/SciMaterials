using Moq;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Repositories.Files;
using SciMaterials.DAL.Resources.Repositories.Users;
using SciMaterials.DAL.Resources.UnitOfWork;

namespace SciMaterials.RepositoryTests.Helpers;

public static class UnitOfWorkHelper
{
    public static Mock<IUnitOfWork<SciMaterialsContext>> GetUnitOfWorkMock()
    {
        var context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<SciMaterialsFilesUnitOfWork>(loggerFactory);

        var unitOfWork = new Mock<IUnitOfWork<SciMaterialsContext>>();
        unitOfWork.Setup(x => x.GetRepository<Category>()).Returns(new CategoryRepository(context, logger));
        unitOfWork.Setup(x => x.GetRepository<Author>()).Returns(new AuthorRepository(context, logger));

        return unitOfWork;
    }
}