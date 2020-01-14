using System.ComponentModel.DataAnnotations;

namespace Gratify.Grats.Api.Database
{
    public class Grats
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Sender { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(500)]
        public string Content { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Approver { get; set; }

        public bool? IsApproved { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Receiver { get; set; }
    }
}
