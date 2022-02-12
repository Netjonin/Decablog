using System;

namespace DecaBlog.Models.DTO
{
    public class ArticlesToReturnByPublisherIdDto
    {
        public string ContributionId { get; set; }
        public string TopicId { get; set; }
        public string Topic { get; set; }
        public string SubTopic { get; set; }
        public string ArticleText { get; set; }
        public string Keywords { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ContributorDto Contributor { get; set; }
    }

    public class ContributorDto
    {
        public string ContributorId { get; set; }
        public string ContributorName { get; set; }
    }
}