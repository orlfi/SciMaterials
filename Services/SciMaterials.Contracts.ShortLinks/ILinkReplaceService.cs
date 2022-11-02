namespace SciMaterials.Contracts.ShortLinks;
public interface ILinkReplaceService
{
    Task<string> RestoreLinks(string text, CancellationToken Cancel);
    Task<string> ShortenLinks(string text, CancellationToken Cancel);
}

