using Microsoft.EntityFrameworkCore;

namespace SciMaterials.LinkSearch.WebAPI.Data
{
    public class LinkSearchDbContextApp : DbContext
    {
        public DbSet<Models.LinkSearch> LinkSearches { get; set; }

        public LinkSearchDbContextApp(DbContextOptions options) : base(options) { }
    }
}