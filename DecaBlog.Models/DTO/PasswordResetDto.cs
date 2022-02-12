using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class PasswordResetDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        [StringLength(40, MinimumLength = 5)]
        public string NewPassword { get; set; }
    }
}
