using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.ShortLinks;

using SciMaterials.DAL.Models;

namespace SciMaterials.Services.ShortLinks;

public class LinkReplaceService : ILinkReplaceService
{
    private const string originalLinkPattern = @"((http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?)";
    private const string shortLinkPattern = @"((http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?)";
    private readonly ILinkShortCutService _linkShortCut;
    private readonly ILogger<LinkReplaceService> _logger;

    public LinkReplaceService(ILinkShortCutService linkShortCut, ILogger<LinkReplaceService> logger)
    {
        _linkShortCut = linkShortCut;
        _logger = logger;
    }

    public async Task<string> ShortenLinks(string text, CancellationToken Cancel)
    {
        var regex = new Regex(originalLinkPattern);
        var sb = new StringBuilder();
        var lastIndex = 0;
        foreach (var match in regex.Matches(text).Cast<Match>())
        {
            _logger.LogDebug("Link found: {link}", match.Value);
            sb.Append(text, lastIndex, match.Index - lastIndex);
            var shortLink = await _linkShortCut.GetOrAddAsync(match.Value, Cancel);
            sb.Append(shortLink);
            lastIndex = match.Index + match.Length;
        }
        sb.Append(text, lastIndex, text.Length - lastIndex);

        return sb.ToString();
    }
    public async Task<string> RestoreLinks(string text, CancellationToken Cancel)
    {
        var regex = new Regex(shortLinkPattern);
        var sb = new StringBuilder();
        var lastIndex = 0;
        foreach (Match match in regex.Matches(text))
        {
            _logger.LogDebug("Short link found: {link}", match.Value);
            sb.Append(text, lastIndex, match.Index - lastIndex);
            var shortLink = await _linkShortCut.GetOrAddAsync(match.Value, Cancel);
            sb.Append(shortLink);
            lastIndex = match.Index + match.Length;
        }
        sb.Append(text, lastIndex, text.Length - lastIndex);

        return sb.ToString();
    }
}

