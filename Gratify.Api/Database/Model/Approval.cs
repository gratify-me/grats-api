using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Approval : Entity
    {
        [Required]
        public DateTime ApprovedAt { get; set; }

        [Required]
        public int ReviewId { get; set; }

        [Required]
        public Review Review { get; set; }
    }
}
