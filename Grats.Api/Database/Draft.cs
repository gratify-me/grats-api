using System.ComponentModel.DataAnnotations;
using Slack.Client.Primitives;

namespace Gratify.Grats.Api.Database
{
    public class Draft
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public User Sender { get; set; }

        [MinLength(1)]
        [MaxLength(500)]
        public string Content { get; set; }

        [Required]
        public bool IsSubmitted { get; set; }

        [MinLength(1)]
        [MaxLength(100)]
        public User Receiver { get; set; }
    }
}
