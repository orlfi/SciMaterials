namespace SciMaterials.Contracts.Result.Codes;

public static partial class ErrorCodes
{
    public static class File
    {
        public static partial class Service
        {
        }

        public static partial class Store
        {
            public const int FileNotFound = 1001;
            public const int FileAlreadyExist = 1002;
        }
    }
}
