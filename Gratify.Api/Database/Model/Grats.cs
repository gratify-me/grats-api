using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Grats : Entity
    {
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Recipient { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(300)]
        public string Challenge { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(300)]
        public string Action { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(300)]
        public string Result { get; set; }

        [Required]
        public int DraftId { get; set; }

        [Required]
        public Draft Draft { get; set; }

        public Review Review { get; set; }
    }
}
