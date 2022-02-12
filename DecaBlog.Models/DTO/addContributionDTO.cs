using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class AddContributionDTO
    {
        public string SubTopic { get; set; } = null;
        [Required]
        public string ArtlcleText { get; set; }

        [StringLength(150, ErrorMessage = "Keyword Maximum length is 150 Characters")]
        public string Keywords { get; set; }

    }
}
