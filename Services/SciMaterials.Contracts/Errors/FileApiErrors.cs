namespace SciMaterials.Contracts.Result.Codes;

public static partial class ApiErrors
{
    public static class File
    {
        public static readonly Error Update = new(1000, "File update failure");
        public static readonly Error Download = new(1001, "File download failure");
        public static readonly Error Delete = new(1002, "File delete failure");
        public static readonly Error NotFound = new(1003, "File not found");
        public static readonly Error AlreadyExist = new(1004, "File already exist");
        public static readonly Error ContentTypeNotFound = new(1005, "File content type not found");
        public static readonly Error CategoriesAreNotSpecified = new(1006, "File categories are not specified");
        public static readonly Error CategoriesNotFound = new(1007, "File category not found");
        public static readonly Error AuthorNotFound = new(1008, "File author not found");
        public static readonly Error Add = new(1009, "File write to database failure");
        public static readonly Error StoreWrite = new(1009, "File write to store failure");
    }
}
