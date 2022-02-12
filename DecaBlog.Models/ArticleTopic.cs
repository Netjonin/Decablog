using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DecaBlog.Models
{
    public class ArticleTopic : BaseEntity
    {
        [Required]
        public string Topic { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Maximum is 150 Character")]
        public string Abstract { get; set; }
        public string PhotoUrl { get; set; }
        public string PublicId { get; set; }
        [StringLength(150, ErrorMessage = "Maximum is 150 Characters")]
        [Column("AuthorId")]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User Author { get; set; }
        public string CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public User User { get; set; }
        // navigation props
        public UserComment UserComment { get; set; }
        public List<Article> ArticleList { get; set; }
    }
}
