using System;

namespace DecaBlog.Models.DTO
{
    public class ArticleInfoDto
    {
        public string Id { get; set; }
        public string TopicId { get; set; }
        public string ArticleText { get; set; }
        public bool IsPublished { get; set; } = false;
        public DateTime DateCreated { get; set; }
    }
}