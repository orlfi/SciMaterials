using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Errors.Api;

/// <summary> Ошибки API (10000-19999). </summary>
public static partial class ApiErrors
{
    /// <summary> Ошибки сервиса TagService (10500-10599). </summary>
    public static class Tag
    {
        public static readonly Error NotFound = new(10500, "Tag not found");
        public static readonly Error Add = new(10501, "Tag add error");
        public static readonly Error Update = new(10502, "Tag update error");
        public static readonly Error Delete = new(10503, "Tag delete error");
        public static readonly Error Exist = new(10504, "Tag already exist");
    }
}
