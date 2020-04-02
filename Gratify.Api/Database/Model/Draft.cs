using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Draft : Entity
    {
        public Draft(Guid correlationId, string teamId, DateTime createdAt, string author)
        {
            CorrelationId = correlationId;
            TeamId = teamId;
            CreatedAt = createdAt;
            Author = author;
        }

        [Required]
        public DateTime CreatedAt { get; private set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Author { get; private set; }

        public Grats Grats { get; set; }
    }
}
