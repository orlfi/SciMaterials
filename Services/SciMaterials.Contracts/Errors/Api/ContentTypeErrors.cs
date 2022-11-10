using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Errors.Api;

/// <summary> Ошибки API (10000-19999). </summary>
public static partial class ApiErrors
{
    /// <summary> Ошибки сервиса ContentTypeService (10400-10499). </summary>
    public static class ContentType
    {
        public static readonly Error NotFound = new(10400, "ContentType not found");
        public static readonly Error Add = new(10401, "ContentType add error");
        public static readonly Error Update = new(10402, "ContentType update error");
        public static readonly Error Delete = new(10403, "ContentType delete error");
        public static readonly Error Exist = new(10404, "ContentType already exist");
    }
}
