using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class EmailConfirmationDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
