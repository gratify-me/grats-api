using Gratify.Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gratify.Api.Database
{
    public class GratsDb : DbContext
    {
        public GratsDb(DbContextOptions<GratsDb> options) : base(options) { }

        public DbSet<Draft> Drafts { get; set; }

        public DbSet<Grats> Grats { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Approval> Approvals { get; set; }

        public DbSet<Denial> Denials { get; set; }

        public DbSet<Receival> Receivals { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
