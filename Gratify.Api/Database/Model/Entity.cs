using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid CorrelationId { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string TeamId { get; set; }
    }
}
