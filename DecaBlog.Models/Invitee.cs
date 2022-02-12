using System;
using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models
{
    public class Invitee
    {
        [Key]
        public string InviteeId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Must be between 3 and 30")]
        public string LastName { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Must be between 3 and 30")]
        public string FirstName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? StackId { get; set; }
        public string? SquadId { get; set; }
        [Required]
        public string InviterToken { get; set; }
        public DateTime InvitedOn { get; set; } = DateTime.Now;
    }
}

