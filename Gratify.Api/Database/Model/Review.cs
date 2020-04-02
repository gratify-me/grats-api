using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Review : Entity
    {
        [Required]
        public DateTime RequestedAt { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Reviewer { get; set; }

        public int ForwardedFrom { get; set; }

        [Required]
        public int GratsId { get; set; }

        [Required]
        public Grats Grats { get; set; }

        public Approval Approval { get; set; }

        public Denial Denial { get; set; }
    }
}
