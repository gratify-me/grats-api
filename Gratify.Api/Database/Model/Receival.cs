using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Receival : Entity
    {
        [Required]
        public DateTime ReceivedAt { get; set; }

        [Required]
        public int ApprovalId { get; set; }

        [Required]
        public Approval Approval { get; set; }
    }
}
