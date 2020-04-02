using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Review : Entity
    {
        public Review(Guid correlationId, DateTime requestedAt, string reviewer)
        {
            CorrelationId = correlationId;
            RequestedAt = requestedAt;
            Reviewer = reviewer;
        }

        [Required]
        public DateTime RequestedAt { get; private set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Reviewer { get; private set; }

        public int? ForwardedFrom { get; set; }

        [Required]
        public int GratsId { get; set; }

        [Required]
        public Grats Grats { get; set; }

        public Approval Approval { get; set; }

        public Denial Denial { get; set; }
    }
}
