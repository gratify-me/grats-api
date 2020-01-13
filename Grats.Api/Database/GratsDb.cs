using Microsoft.EntityFrameworkCore;

namespace Gratify.Grats.Api.Database
{
    public class GratsDb : DbContext
    {
        public GratsDb() { }

        public GratsDb(DbContextOptions<GratsDb> options) : base(options) { }

        public DbSet<Grats> Grats { get; set; }
    }
}
