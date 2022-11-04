namespace SciMaterials.Contracts.ShortLinks;
public interface ILinkReplaceService
{
    Task<string> RestoreLinks(string text, CancellationToken Cancel = default);
    Task<string> ShortenLinks(string text, CancellationToken Cancel = default);
}

