
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SciMaterials.DAL.Contexts;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Helpers;

public class SciMateralsContextHelper
{
    public SciMaterialsContext Context { get; set; }

    public SciMateralsContextHelper()
    {
        var builder = new DbContextOptionsBuilder<SciMaterialsContext>();
        builder.UseInMemoryDatabase("SQLITE_UNIT_TESTNGS")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

        var options = builder.Options;
        Context = new SciMaterialsContext(options);

        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        Context.AddRange(CategoryHelper.GetMany());
        //Context.AddRange(BrandModelHelper.GetMany());
        //Context.AddRange(ConainerTypeModelHelper.GetMany());

        //var addresses = AddressModelHelper.GetMany();
        //Context.AddRange(addresses);

        //var containers = ContainerModelHalper.GetMany();
        //Context.AddRange(containers);

        //Context.AddRange(ProductTypeModelHelper.GetMany());

        //var products = ProductModelHelper.GetMany();
        //Context.AddRange(products);

        //var warehouses = WarehouseModelHelper.GetMany();
        //Context.AddRange(warehouses);

        //var storProd = StoredProductModelHelper.GetOne();
        //storProd.ActualContainerModelId = containers.First().Id;
        //storProd.AddressModelId = addresses.First().Id;
        //storProd.ProductModelId = products.First().Id;
        //storProd.WarehouseModelId = warehouses.First().Id;
        //Context.Add(storProd);


        Context.SaveChanges();
    }
}