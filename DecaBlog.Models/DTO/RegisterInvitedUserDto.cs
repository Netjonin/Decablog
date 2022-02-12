using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class RegisterInvitedUserDto
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Must be between 3 and 15")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Must be between 3 and 15")]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        [DisplayName("Stack")]
        public string StackId { get; set; }
        [Required]
        [DisplayName("Squad")]
        public string SquadId { get; set; }
    }
}
