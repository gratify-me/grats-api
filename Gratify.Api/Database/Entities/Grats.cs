using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Grats : Entity
    {
        public Grats(Guid correlationId, DateTime createdAt, string recipient, string challenge, string action, string result)
        {
            CorrelationId = correlationId;
            CreatedAt = createdAt;
            Recipient = recipient;
            Challenge = challenge;
            Action = action;
            Result = result;
        }

        [Required]
        public DateTime CreatedAt { get; private set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Recipient { get; private set; }

        [Required]
        [MinLength(1)]
        [MaxLength(300)]
        public string Challenge { get; private set; }

        [Required]
        [MinLength(1)]
        [MaxLength(300)]
        public string Action { get; private set; }

        [Required]
        [MinLength(1)]
        [MaxLength(300)]
        public string Result { get; private set; }

        [Required]
        public int DraftId { get; set; }

        [Required]
        public Draft Draft { get; set; }

        public ICollection<Review> Reviews { get; } = new List<Review>();
    }
}
