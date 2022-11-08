namespace SciMaterials.Contracts.Result.Codes;

public static partial class ErrorCodes
{
    public static partial class File
    {
        public static class Service
        {
            public const int FileDownloadError = 1001;
            public const int FileDeleteError = 1002;
            public const int FileAlreadyExist = 1012;
        }
    }
}
