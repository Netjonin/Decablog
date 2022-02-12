using System;

namespace DecaBlog.Models.DTO
{
    public class ArticlesToReturnByContributorIdDto
    {
        public string ContributionId { get; set; }
        public string TopicId { get; set; }
        public string Topic { get; set; }
        public string SubTopic { get; set; }
        public string ArticleText { get; set; }
        public string Keywords { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PublisherDto Publisher { get; set; }
    }

    public class PublisherDto
    {
        public string PublisherId { get; set; }
        public string PublisherBy { get; set; }
    }
}