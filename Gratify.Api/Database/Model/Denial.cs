using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Denial : Entity
    {
        [Required]
        public DateTime DeniedAt { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(500)]
        public string Reason { get; set; }

        [Required]
        public int ReviewId { get; set; }

        [Required]
        public Review Review { get; set; }
    }
}
