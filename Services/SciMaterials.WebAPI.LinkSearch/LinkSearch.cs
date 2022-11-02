using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.WebAPI.LinkSearch;
using SciMaterials.DAL.Models;

namespace SciMaterials.WebAPI.LinkSearch;

public class LinkSearch : ILinkReplace
{
    private const string _pattern = @"((http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?)";
    private readonly ILinkShortCut<Link> _linkShortCut;
    private readonly ILogger<LinkSearch> _logger;

    public LinkSearch(ILinkShortCut<Link> linkShortCut, ILogger<LinkSearch> logger)
    {
        _linkShortCut = linkShortCut;
        _logger = logger;
    }

    public async Task<string> ShortenLinks(string text, CancellationToken Cancel)
    {
        var regex = new Regex(_pattern);
        var sb = new StringBuilder();
        var lastIndex = 0;
        foreach (Match match in regex.Matches(text))
        {
            _logger.LogDebug(match.Value);
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
        var regex = new Regex(_pattern);
        var sb = new StringBuilder();
        var lastIndex = 0;
        foreach (Match match in regex.Matches(text))
        {
            _logger.LogDebug(match.Value);
            sb.Append(text, lastIndex, match.Index - lastIndex);
            var shortLink = await _linkShortCut.GetOrAddAsync(match.Value, Cancel);
            sb.Append(shortLink);
            lastIndex = match.Index + match.Length;
        }
        sb.Append(text, lastIndex, text.Length - lastIndex);

        return sb.ToString();
    }
}