using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Errors.Api;

/// <summary> Ошибки API (10000-19999). </summary>
public static partial class ApiErrors
{
    /// <summary> Ошибки сервиса CommentService (10100-10199). </summary>
    public static class Comment
    {
        public static readonly Error NotFound = new(10100, "Comment not found");
        public static readonly Error Add = new(10101, "Comment add error");
        public static readonly Error Update = new(10102, "Comment update error");
        public static readonly Error Delete = new(10103, "Comment delete error");
        public static readonly Error Exist = new(10104, "Comment already exist");
    }
}
