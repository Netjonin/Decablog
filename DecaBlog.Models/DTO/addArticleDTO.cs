using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class AddArticleDTO
    {
        [Required]
        public IFormFile Photo { get; set; }
        [Required]
        public string Topic { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Maximum is 150 Character")]
        public string Abstract { get; set; }
        [Required]
        public string CategoryId { get; set; }
        public string SubTopic { get; set; }
        [Required]
        public string ArtlcleText { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Keyword Maximum length is 150 Characters")]
        public string Keywords { get; set; }
    }
}
