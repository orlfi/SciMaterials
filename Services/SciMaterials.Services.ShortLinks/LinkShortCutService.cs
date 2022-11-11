using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SciMaterials.Contracts.Errors;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.Contracts.ShortLinks.Settings;
using SciMaterials.DAL.Contexts;

namespace SciMaterials.Services.ShortLinks;

public class LinkShortCutService : ServiceBase, ILinkShortCutService
{
    private readonly SciMaterialsContext _db;
    private readonly LinkGenerator _linkGenerator;
    private readonly HttpContext _httpContext;

    private readonly string _encodingName;
    private readonly int _hashLength;
    private readonly string _hashAlgorithmName;

    public LinkShortCutService(
        SciMaterialsContext db,
        IOptions<LinkShortCutOptions> options,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor,
        ILogger<LinkShortCutService> logger) : base(logger)
    {
        _db = db;
        _linkGenerator = linkGenerator;
        _httpContext = httpContextAccessor.HttpContext;

        (
            _hashLength,
            _hashAlgorithmName,
            _encodingName
        ) = options.Value;
    }

    /// <summary>
    /// Добавляет хеш новой ссылки.
    /// Если ссылка уже существует, то возвращает хеш.
    /// </summary>
    /// <param name="sourceAddress"> Адресс ссылки.</param>
    /// <param name="Cancel"> Токен отмены. </param>
    /// <returns> Сокращенный хеш. </returns>
    public async Task<Result<string>> AddAsync(string sourceAddress, CancellationToken Cancel = default)
    {
        var hash = await ComputeHash(sourceAddress, Cancel);
        if (await _db.Links.FirstOrDefaultAsync(l => l.Hash == hash, Cancel) is not { } link)
        {
            link = new() { SourceAddress = sourceAddress, Hash = hash };
            _db.Add(link);
            await _db.SaveChangesAsync(Cancel);
        }

        var shortLink = await GetMinShortLinkAsync(link.Hash, _hashLength, Cancel);
        _logger.LogInformation("ShortLink {shortLink} for adress {sourceAddress} added", shortLink, sourceAddress);
        return shortLink;
    }

    /// <summary> Возвращает адрес исходной ссылки по хешу. </summary>
    /// <param name="hash"> Сокращенный хеш. </param>
    /// <param name="Cancel"> Токен отмены. </param>
    /// <returns> Адрес исходной ссылки. </returns>
    public Task<Result<string>> GetAsync(string hash, CancellationToken Cancel = default)
    {
        return Task.FromResult(Result<string>.Success("testlink"));
    }

    private async Task<Result<string>> GetMinShortLinkAsync(string hash, int linkLength, CancellationToken Cancel = default)
    {
        var shortLink = hash[..linkLength];

        var linksCount = await _db.Links.CountAsync(item => item.Hash.StartsWith(shortLink), Cancel);
        switch (linksCount)
        {
            case 0:
                return LoggedError<string>(
                    Errors.ShortLink.HashNotFound,
                    "Link with hash {hash} not found",
                    hash);
            case 1:
                var address = _linkGenerator.GetUriByAction(_httpContext, "GetById", "Links", new { hash = shortLink }, _httpContext.Request.Scheme);
                _logger.LogDebug("Found short link with minimal length: {address}", address);
                return address;
        }

        return await GetMinShortLinkAsync(hash, linkLength++, Cancel);
    }

    private async Task<string> ComputeHash(string text, CancellationToken Cancel = default)
    {
        var encoding = Encoding.GetEncoding(_encodingName);
        var bytes = new MemoryStream(encoding.GetBytes(text));

        var hasher = HashAlgorithm.Create(_hashAlgorithmName)
                     ?? throw new InvalidOperationException(
                         $"Failed to retrieve hashing algorithm {_hashAlgorithmName}");
        var hashBytes = await hasher.ComputeHashAsync(bytes, Cancel).ConfigureAwait(false);
        var hash = Convert.ToBase64String(hashBytes);
        return hash;
    }
}