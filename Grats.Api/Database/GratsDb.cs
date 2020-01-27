using Microsoft.EntityFrameworkCore;

namespace Gratify.Grats.Api.Database
{
    public class GratsDb : DbContext
    {
        public GratsDb(DbContextOptions<GratsDb> options) : base(options) { }

        public DbSet<Draft> Drafts { get; set; }

        public DbSet<Grats> Grats { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
