using System;
using System.Linq;
using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gratify.Api.Database
{
    public class GratsDb : DbContext
    {
        public GratsDb(DbContextOptions<GratsDb> options) : base(options) { }

        public DbSet<Grats> Grats { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public IQueryable<Review> IncompleteReviews =>
            Reviews
                .Include(review => review.Grats)
                .Where(review => !review.IsForwarded)
                .Where(review => review.Approval == null)
                .Where(review => review.Denial == null);

        public DbSet<Approval> Approvals { get; set; }

        public DbSet<Denial> Denials { get; set; }

        public DbSet<Receival> Receivals { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Settings> Settings { get; set; }

        public async Task<Settings> SettingsFor(string teamId, string userId)
        {
            // TODO: Settings should probably be created on installation. In addition we might want to cache settings.
            var maybeSettings = await Settings.SingleOrDefaultAsync(settings => settings.TeamId == teamId);
            if (maybeSettings != default)
            {
                return maybeSettings;
            }

            var defaultSettings = new Settings(
                teamId: teamId,
                createdAt: DateTime.UtcNow,
                createdBy: userId);

            await Settings.AddAsync(defaultSettings);
            await SaveChangesAsync();

            return defaultSettings;
        }

        public IQueryable<Grats> PendingAndApprovedGratsFor(string userId, int gratsPeriodInDays) =>
            Grats
                .Where(grats => grats.Author == userId)
                .Where(grats => grats.CreatedAt > DateTime.UtcNow.AddDays(-gratsPeriodInDays))
                .Where(grats =>
                    !grats.Reviews.Any()
                    || grats.Reviews.Any(review => review.Approval != null)
                    || grats.Reviews.All(review => review.Approval == null && review.Denial == null))
                .OrderByDescending(grats => grats.CreatedAt);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Settings>()
                .HasIndex(b => b.TeamId)
                .IsUnique();
        }
    }
}
