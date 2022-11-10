using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Errors;

public static partial class MiddlewareErrors
{
    public static class Exception
    {
        public static readonly Error Unhandled = new(1000, "Server unhandled error");
    }
}
