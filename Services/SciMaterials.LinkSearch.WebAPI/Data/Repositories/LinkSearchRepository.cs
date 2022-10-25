using Microsoft.EntityFrameworkCore;

using SciMaterials.LinkSearch.WebAPI.Data.Interfaces;

namespace SciMaterials.LinkSearch.WebAPI.Data.Repositories
{
    public class LinkSearchRepository : ILinkSearch
    {
        private readonly LinkSearchDbContextApp _db;
        private readonly ILogger<LinkSearchRepository> _logger;

        public LinkSearchRepository(LinkSearchDbContextApp db, ILogger<LinkSearchRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Models.LinkSearch>> GetAllAsync(CancellationToken cancel = default)
        {
            var link = await _db.LinkSearches.ToListAsync();
            return link;
        }

        public async Task<Models.LinkSearch> GetByIdAsync(int id, CancellationToken cancel = default)
        {
            var link = await _db.LinkSearches.FirstOrDefaultAsync(x => x.Id == id);

            if (id == 0)
                _logger.LogInformation("Id Not Found");

            return link;
        }

        public async Task CreateAsync(Models.LinkSearch data, CancellationToken cancel = default)
        {
            var link = await _db.LinkSearches.FirstOrDefaultAsync(x => x.Id == data.Id);
            if (link == null)
                _logger.LogError("Not Found");
            
            await _db.AddAsync(data);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, CancellationToken cancel = default)
        {
            var link = await _db.LinkSearches.FirstOrDefaultAsync(x => x.Id == id);
            if (id == 0)
                _logger.LogInformation("Id Not Found");

            _db.Remove(link);
            await _db.SaveChangesAsync();
        }
    }
}