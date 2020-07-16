using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Grats : Entity
    {
        public Grats(Guid correlationId, string teamId, DateTime createdAt, string author, string recipient, string challenge, string action, string result)
        {
            CorrelationId = correlationId;
            TeamId = teamId;
            CreatedAt = createdAt;
            Author = author;
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
        public string Author { get; private set; }

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

        public ICollection<Review> Reviews { get; } = new List<Review>();
    }
}
