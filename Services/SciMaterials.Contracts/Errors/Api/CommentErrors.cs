using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Errors.Api;

/// <summary> Ошибки API (10000-19999). </summary>
public static partial class ApiErrors
{
    /// <summary> Ошибки сервиса CommentService (10300-10399). </summary>
    public static class Comment
    {
        public static readonly Error NotFound = new(10300, "Comment not found");
        public static readonly Error Add = new(10301, "Comment add error");
        public static readonly Error Update = new(10302, "Comment update error");
        public static readonly Error Delete = new(10303, "Comment delete error");
        public static readonly Error Exist = new(10304, "Comment already exist");
    }
}
