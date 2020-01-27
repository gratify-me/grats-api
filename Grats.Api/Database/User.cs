using System.ComponentModel.DataAnnotations;

namespace Gratify.Grats.Api.Database
{
    public class User
    {
        [Key]
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Id { get; set; }

        [MaxLength(100)]
        public string GratsApprover { get; set; }
    }
}
