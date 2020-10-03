using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Settings
    {
        public Settings(string teamId, DateTime createdAt, string createdBy)
        {
            TeamId = teamId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            UpdatedAt = createdAt;
            UpdatedBy = createdBy;
            GratsPeriodInDays = 30;
            NumberOfGratsPerPeriod = 1;
            AmountPerGrats = 1500;
        }

        [Key]
        public int Id { get; private set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string TeamId { get; private set; }

        [Required]
        public DateTime CreatedAt { get; private set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string CreatedBy { get; private set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string UpdatedBy { get; set; }

        [Required]
        [Range(0, 365)]
        public int GratsPeriodInDays { get; set; }

        [Required]
        [Range(0, 100)]
        public int NumberOfGratsPerPeriod { get; set; }

        [Required]
        [Range(0, 10000)]
        public int AmountPerGrats { get; set; }
    }
}
