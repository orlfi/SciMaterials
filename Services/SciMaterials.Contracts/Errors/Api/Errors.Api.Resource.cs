using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts;

public static partial class Errors
{
    /// <summary> Ошибки API (10000-19999). </summary>
    public static partial class Api
    {
        /// <summary> Ошибки сервиса ResourceService (API700-API799). </summary>
        public static class Resource
        {
            public static readonly Error NotFound = new("API700", "Resource not found");
        }
    }
}
