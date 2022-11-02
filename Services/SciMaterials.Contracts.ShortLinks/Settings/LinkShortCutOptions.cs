namespace SciMaterials.Contracts.ShortLinks.Settings;

public record LinkShortCutOptions
(
    int HashStringLength = 5,
    string HashAlgorithm = "SHA512",
    string Encoding = "UTF-32"
)
{
    public LinkShortCutOptions() : this(5, "SHA512", "UTF-32") { }
}