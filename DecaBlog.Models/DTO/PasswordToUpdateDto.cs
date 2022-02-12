using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class PasswordToUpdateDto
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
