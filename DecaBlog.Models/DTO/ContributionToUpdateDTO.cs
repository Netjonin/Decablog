using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class ContributionToUpdateDTO
    {
        public string SubTopic { get; set; } = null;
        [Required]
        public string ArtlcleText { get; set; }
        public string Abstract { get; set; }
        [StringLength(150, ErrorMessage = "Keyword Maximum length is 150 Characters")]
        public string Keywords { get; set; }
    }
}
