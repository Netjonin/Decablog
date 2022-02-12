using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class AddArticleTopicDTO
    {
        [Required]
        public string Topic { get; set; }
        [Required]
        public string TopicId { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Maximum is 150 Character")]
        public string Abstract { get; set; }
        public string PhotoUrl { get; set; }
        public string PublicId { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Maximum is 150 Character")]
        public string Category { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Maximum is 150 Characters")]
        public string AuthorId { get; set; }
    }
}
