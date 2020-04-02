using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class User : Entity
    {
        public User(string teamId, string userId, string defaultReviewer = null, bool hasReports = false)
        {
            TeamId = teamId;
            UserId = userId;
            DefaultReviewer = defaultReviewer;
            HasReports = hasReports;
        }

        [MinLength(1)]
        [MaxLength(100)]
        public string UserId { get; private set; }

        [MaxLength(100)]
        public string DefaultReviewer { get; set; }

        [Required]
        public bool HasReports { get; set; }
    }
}
