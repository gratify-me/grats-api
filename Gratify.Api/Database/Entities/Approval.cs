using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Approval : Entity
    {
        public Approval(Guid correlationId, DateTime approvedAt)
        {
            CorrelationId = correlationId;
            ApprovedAt = approvedAt;
        }

        [Required]
        public DateTime ApprovedAt { get; private set; }

        [Required]
        public int ReviewId { get; set; }

        [Required]
        public Review Review { get; set; }

        [Required]
        public bool IsNotified { get; set; }
    }
}
