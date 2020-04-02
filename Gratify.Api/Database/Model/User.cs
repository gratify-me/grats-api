using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class User : Entity
    {
        [MinLength(1)]
        [MaxLength(100)]
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DefaultReviewer { get; set; }

        [Required]
        public bool HasReports { get; set; }
    }
}
