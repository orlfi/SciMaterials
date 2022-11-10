using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Errors.Api;

/// <summary> Ошибки API (10000-19999). </summary>
public static partial class ApiErrors
{
    /// <summary> Ошибки сервиса FileService (10000-10099). </summary>
    public static class File
    {
        public static readonly Error NotFound = new(10000, "File not found");
        public static readonly Error Add = new(10001, "File add to database error");
        public static readonly Error Update = new(10002, "File update error");
        public static readonly Error Delete = new(10003, "File delete error");
        public static readonly Error Download = new(10004, "File download error");
        public static readonly Error Exist = new(10005, "File already exist");
        public static readonly Error ContentTypeNotFound = new(10006, "File content type not found");
        public static readonly Error CategoriesAreNotSpecified = new(10007, "File categories are not specified");
        public static readonly Error CategoriesNotFound = new(10008, "File category not found");
        public static readonly Error AuthorNotFound = new(10009, "File author not found");
        public static readonly Error StoreWrite = new(10010, "File write to store error");
    }
}
