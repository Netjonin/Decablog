using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DecaBlog.Models
{
    public class Article : BaseEntity
    {

        public string SubTopic { get; set; }
        [Required]
        public string ArticleText { get; set; }
        [MaxLength(150, ErrorMessage = "Keyword Maximum length is 150 Characters")]
        public string Keywords { get; set; }
        public bool IsPublished { get; set; } = false;
        [Column("ContributorId")]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User Contributor { get; set; }
        public string PublisherId { get; set; }
        public string ArticleTopicId { get; set; }
        [ForeignKey("ArticleTopicId")]
        public ArticleTopic ArticleTopic { get; set; }
    }
}
