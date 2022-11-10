using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Errors.Api;

/// <summary> Ошибки API (10000-19999). </summary>
public static partial class ApiErrors
{
    /// <summary> Ошибки сервиса UrlService (10600-10699). </summary>
    public static class Url
    {
        public static readonly Error NotFound = new(10600, "Url not found");
        public static readonly Error Add = new(10601, "Url add error");
        public static readonly Error Update = new(10602, "Url update error");
        public static readonly Error Delete = new(10603, "Url delete error");
        public static readonly Error Exist = new(10604, "Url already exist");
    }
}
