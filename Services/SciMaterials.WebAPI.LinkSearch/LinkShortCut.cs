using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SciMaterials.Contracts.WebAPI.LinkSearch;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.WebAPI.LinkSearch.Options;

namespace SciMaterials.WebAPI.LinkSearch
{
    public class LinkShortCut : ILinkShortCut<Link>
    {
        private static readonly Regex __Regex = new (@"^(?<scheme>[A-z]+)://", RegexOptions.Compiled);
        private readonly SciMaterialsContext _db;
        private readonly ILogger<LinkShortCut> _logger;
        
        private readonly string _encodingName;
        private readonly int    _hashLength;
        private readonly string _hashAlgorithmName;

        public LinkShortCut(SciMaterialsContext db, ILogger<LinkShortCut> logger, IOptions<LinkShortCutOptions> options)
        {
            _db = db;
            _logger = logger;

            (
                _hashLength,
                _hashAlgorithmName,
                _encodingName
            ) = options.Value;
        }

        public async Task<string> AddAsync(string sourceAddress, CancellationToken cancel = default)
        {
            if (!__Regex.IsMatch(sourceAddress))
                sourceAddress = "http://" + sourceAddress;

            var encoding = Encoding.GetEncoding(_encodingName);
            var bytes = new MemoryStream(encoding.GetBytes(sourceAddress));

            var hasher = HashAlgorithm.Create(_hashAlgorithmName)
                         ?? throw new InvalidOperationException(
                             $"Failed to retrieve hashing algorithm {_hashAlgorithmName}");
            var hashBytes = await hasher.ComputeHashAsync(bytes, cancel).ConfigureAwait(false);
            var hash = Convert.ToBase64String(hashBytes);
            
            if (await _db.Links.FirstOrDefaultAsync(l => l.Hash == hash, cancel) is { } link)
                _logger.LogInformation("A record of the {0} exists in the database with the hash {1}.", sourceAddress, link.Hash);
            else
            {
                link = new() { SourceAddress = sourceAddress, Hash = hash };
                
                _db.Add(link);
                await _db.SaveChangesAsync(cancel);
            
                _logger.LogInformation("Record {0} added to database with hash {1}", sourceAddress, hash);
            }

            return hash[..Math.Min(hash.Length, _hashLength)];
        }

        public async Task<Link> FindByHashAsync(string hash, CancellationToken cancel = default)
        {
            var link = await _db.Links.FirstOrDefaultAsync(l => l.Hash == hash, cancel);
            if (link == null)
                _logger.LogInformation("Hash not found.");

            return link;
        }

        public async Task<Link> FindByUrlAsync(string sourceAddress, CancellationToken cancel = default)
        {
            if (!__Regex.IsMatch(sourceAddress))
                sourceAddress = "http://" + sourceAddress;
            var link = await _db.Links.FirstOrDefaultAsync(l => l.SourceAddress == sourceAddress, cancel);
            if (link == null)
                _logger.LogInformation("Url not found.");

            return link;
        }

        public async Task<IEnumerable<Link>> GetAllAsync(CancellationToken cancel = default)
        {
            return await _db.Links.ToListAsync(cancel);
        }

        public async Task DeleteByHashAsync(string hash, CancellationToken cancel = default)
        {
            var link = await _db.Links.FirstOrDefaultAsync(l => l.Hash == hash, cancel);
            if (link == null)
                _logger.LogInformation("Hash not found.");

            _db.Remove(link);
            await _db.SaveChangesAsync(cancel);
            _logger.LogInformation("Record deleted");
        }

        public async Task DeleteByUrlAsync(string sourceAddress, CancellationToken cancel = default)
        {
            var link = await _db.Links.FirstOrDefaultAsync(l => l.SourceAddress == sourceAddress, cancel);
            if (link == null)
                _logger.LogInformation("Url not found.");

            _db.Remove(link);
            await _db.SaveChangesAsync(cancel);
            _logger.LogInformation("Record deleted");
        }
    }
}