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
            IsForwarded = false;
        }

        public Review(Guid correlationId, DateTime requestedAt, string reviewer, Grats grats, string teamId, int forwardedFrom)
        {
            CorrelationId = correlationId;
            RequestedAt = requestedAt;
            Reviewer = reviewer;
            IsForwarded = false;
            Grats = grats;
            TeamId = teamId;
            ForwardedFrom = forwardedFrom;
        }

        [Required]
        public DateTime RequestedAt { get; private set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Reviewer { get; private set; }

        public int? ForwardedFrom { get; private set; }

        [Required]
        public bool IsForwarded { get; private set; }

        [Required]
        public int GratsId { get; set; }

        [Required]
        public Grats Grats { get; set; }

        public Approval Approval { get; set; }

        public Denial Denial { get; set; }

        public Review ForwardTo(string newReviewerId)
        {
            IsForwarded = true;

            return new Review(
                correlationId: CorrelationId,
                requestedAt: DateTime.UtcNow,
                reviewer: newReviewerId,
                grats: Grats,
                teamId: TeamId,
                forwardedFrom: Id);
        }
    }
}
