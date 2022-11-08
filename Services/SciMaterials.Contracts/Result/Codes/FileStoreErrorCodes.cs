namespace SciMaterials.Contracts.Result.Codes;

public static partial class ErrorCodes
{
    public static partial class File
    {
        public static class Store
        {
            public const int FileNotFound = 1011;
            public const int FileDeletionFailure= 1013;
            public const int FileSavingFailure = 1014;
        }
    }
}
