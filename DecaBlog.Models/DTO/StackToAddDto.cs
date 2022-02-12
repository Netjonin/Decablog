using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class StackToAddDto
    {
        [Required]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Must be between 3 to 10")]
        public string Name { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Must be between 3 to 150")]
        public string Description { get; set; }
    }
}
