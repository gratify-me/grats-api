using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Draft : Entity
    {
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Author { get; set; }

        public Grats Grats { get; set; }
    }
}
