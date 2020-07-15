using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Receival : Entity
    {
        public Receival(Guid correlationId, DateTime receivedAt)
        {
            CorrelationId = correlationId;
            ReceivedAt = receivedAt;
        }

        [Required]
        public DateTime ReceivedAt { get; private set; }

        [Required]
        public int ApprovalId { get; set; }

        [Required]
        public Approval Approval { get; set; }
    }
}
