using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Errors;

public static partial class ShortLinkErrors
{
    public static class ShortCut
    {
        public static readonly Error HashNotFound = new(1100, "Link hash not found");
    }
}
