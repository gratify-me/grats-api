using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class User : Entity
    {
        public User(string teamId, string userId, bool isEligibleForGrats, bool isAdministrator, long updatedAt, string defaultReviewer = null, bool hasReports = false)
        {
            TeamId = teamId;
            UserId = userId;
            IsEligibleForGrats = isEligibleForGrats;
            IsAdministrator = isAdministrator;
            UpdatedAt = updatedAt;
            DefaultReviewer = defaultReviewer;
            HasReports = hasReports;
        }

        [MinLength(1)]
        [MaxLength(100)]
        public string UserId { get; private set; }

        [Required]
        public bool IsEligibleForGrats { get; set; }

        [Required]
        public bool IsAdministrator { get; set; }

        [Required]
        public long UpdatedAt { get; set; }

        [MaxLength(100)]
        public string DefaultReviewer { get; set; }

        [Required]
        public bool HasReports { get; set; }
    }
}
