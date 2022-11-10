namespace SciMaterials.Contracts.Result.Codes;

public static partial class ApiErrors
{
    public static class Link
    {
        public static readonly Error NotFound = new(1003, "Link not found");
    }
}
