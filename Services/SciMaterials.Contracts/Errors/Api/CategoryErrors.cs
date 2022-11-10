using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Errors.Api;

/// <summary> Ошибки API (10000-19999). </summary>
public static partial class ApiErrors
{
    /// <summary> Ошибки сервиса CategoryService (10200-10299). </summary>
    public static class Category
    {
        public static readonly Error NotFound = new(10200, "Category not found");
        public static readonly Error Add = new(10201, "Category add error");
        public static readonly Error Update = new(10202, "Category update error");
        public static readonly Error Delete = new(10203, "Category delete error");
        public static readonly Error Exist = new(10204, "Category already exist");
    }
}
