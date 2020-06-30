using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Denial : Entity
    {
        public Denial(Guid correlationId, DateTime deniedAt, string reason)
        {
            CorrelationId = correlationId;
            DeniedAt = deniedAt;
            Reason = reason;
        }

        [Required]
        public DateTime DeniedAt { get; private set; }

        [Required]
        [MinLength(1)]
        [MaxLength(500)]
        public string Reason { get; private set; }

        [Required]
        public int ReviewId { get; set; }

        [Required]
        public Review Review { get; set; }

        [Required]
        public bool IsNotified { get; set; }
    }
}
