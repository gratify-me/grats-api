using System.Linq;
using Gratify.Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gratify.Api.Database
{
    public class GratsDb : DbContext
    {
        public GratsDb(DbContextOptions<GratsDb> options) : base(options) { }

        public DbSet<Draft> Drafts { get; set; }

        public IQueryable<Draft> IncompleteDrafts =>
            Drafts.Where(draft => draft.Grats == null);

        public DbSet<Grats> Grats { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public IQueryable<Review> IncompleteReviews =>
            Reviews
                .Include(review => review.Grats)
                .ThenInclude(grats => grats.Draft)
                .Where(review => !review.IsForwarded)
                .Where(review => review.Approval == null)
                .Where(review => review.Denial == null);

        public DbSet<Approval> Approvals { get; set; }

        public DbSet<Denial> Denials { get; set; }

        public DbSet<Receival> Receivals { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
