using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Errors;

public static partial class Errors
{
    public static class ShortLink
    {
            public static readonly Error HashNotFound = new("SRV000", "Link hash not found");
    }
}
