using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.Contracts.ShortLinks.Settings;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;

namespace SciMaterials.Services.ShortLinks;

public class LinkShortCutService : ILinkShortCutService
{
    private readonly SciMaterialsContext _db;
    private readonly ILogger<LinkShortCutService> _logger;

    private readonly string _encodingName;
    private readonly int _hashLength;
    private readonly string _hashAlgorithmName;

    public LinkShortCutService(SciMaterialsContext db, IOptions<LinkShortCutOptions> options, ILogger<LinkShortCutService> logger)
    {
        _db = db;
        _logger = logger;

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
        return shortLink;
    }

    /// <summary> Возвращает адрес исходной ссылки по хешу. </summary>
    /// <param name="hash"> Сокращенный хеш. </param>
    /// <param name="Cancel"> Токен отмены. </param>
    /// <returns> Адрес исходной ссылки. </returns>
    public async Task<Result<string>> GetAsync(string hash, CancellationToken Cancel = default)
    {
        return null;
    }

    private async Task<Result<string>> GetMinShortLinkAsync(string hash, int linkLength, CancellationToken Cancel = default)
    {
        var shortLink = hash[..linkLength];
        switch (await _db.Links.CountAsync(item => item.Hash.StartsWith(shortLink), Cancel))
        {
            case 0:
                _logger.LogError("Link with hash {hash} not found", hash);
                return await Result<string>.ErrorAsync((int)ResultCodes.NotFound, $"Link with hash {hash} not found");

            case 1: return $"/{WebApiRoute.ShortLinks}/{shortLink}";
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

    // public async Task<string> AddAsync(string sourceAddress, CancellationToken cancel = default)
    // {
    //     if (!__Regex.IsMatch(sourceAddress))
    //         sourceAddress = "http://" + sourceAddress;

    //     var encoding = Encoding.GetEncoding(_encodingName);
    //     var bytes = new MemoryStream(encoding.GetBytes(sourceAddress));

    //     var hasher = HashAlgorithm.Create(_hashAlgorithmName)
    //                  ?? throw new InvalidOperationException(
    //                      $"Failed to retrieve hashing algorithm {_hashAlgorithmName}");
    //     var hashBytes = await hasher.ComputeHashAsync(bytes, cancel).ConfigureAwait(false);
    //     var hash = Convert.ToBase64String(hashBytes);

    //     if (await _db.Links.FirstOrDefaultAsync(l => l.Hash == hash, cancel) is { } link)
    //         _logger.LogInformation("A record of the {0} exists in the database with the hash {1}.", sourceAddress, link.Hash);
    //     else
    //     {
    //         link = new() { SourceAddress = sourceAddress, Hash = hash };

    //         _db.Add(link);
    //         await _db.SaveChangesAsync(cancel);

    //         _logger.LogInformation("Record {0} added to database with hash {1}", sourceAddress, hash);
    //     }

    //     return hash[..Math.Min(hash.Length, _hashLength)];
    // }

    // public async Task<Link> FindByHashAsync(string hash, CancellationToken cancel = default)
    // {
    //     var link = await _db.Links.FirstOrDefaultAsync(l => l.Hash == hash, cancel);
    //     if (link == null)
    //         _logger.LogInformation("Hash not found.");

    //     return link;
    // }

    // public async Task<Link> FindByUrlAsync(string sourceAddress, CancellationToken cancel = default)
    // {
    //     if (!__Regex.IsMatch(sourceAddress))
    //         sourceAddress = "http://" + sourceAddress;
    //     var link = await _db.Links.FirstOrDefaultAsync(l => l.SourceAddress == sourceAddress, cancel);
    //     if (link == null)
    //         _logger.LogInformation("Url not found.");

    //     return link;
    // }

    // public async Task<IEnumerable<Link>> GetAllAsync(CancellationToken cancel = default)
    // {
    //     return await _db.Links.ToListAsync(cancel);
    // }

    // public async Task DeleteByHashAsync(string hash, CancellationToken cancel = default)
    // {
    //     var link = await _db.Links.FirstOrDefaultAsync(l => l.Hash == hash, cancel);
    //     if (link == null)
    //         _logger.LogInformation("Hash not found.");

    //     _db.Remove(link);
    //     await _db.SaveChangesAsync(cancel);
    //     _logger.LogInformation("Record deleted");
    // }

    // public async Task DeleteByUrlAsync(string sourceAddress, CancellationToken cancel = default)
    // {
    //     var link = await _db.Links.FirstOrDefaultAsync(l => l.SourceAddress == sourceAddress, cancel);
    //     if (link == null)
    //         _logger.LogInformation("Url not found.");

    //     _db.Remove(link);
    //     await _db.SaveChangesAsync(cancel);
    //     _logger.LogInformation("Record deleted");
    // }
}