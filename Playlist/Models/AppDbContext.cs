using Microsoft.EntityFrameworkCore;

namespace Playlist.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<Song> Songs { get; set; }
    }
}
