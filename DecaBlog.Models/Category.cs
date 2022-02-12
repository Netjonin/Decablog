using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models
{
    public class Category
    {
        public Category()
        {
            ArticleTopics = new List<ArticleTopic>();
        }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [StringLength(50, ErrorMessage = "Name Maximum lenght is 50")]
        public string Name { get; set; }
        public List<ArticleTopic> ArticleTopics { get; set; }
    }
}
