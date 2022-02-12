using System;

namespace DecaBlog.Models.DTO
{
    public class PublishedContributionToReturnDto
    {
        public string ArticleText { get; set; }
        public string ArticleId { get; set; }
        public string ContributorId { get; set; }
        public string TopicId { get; set; }
        public string Topic { get; set; }
        public string Subtopic { get; set; }
        public string Keywords { get; set; }
        public PublisherToReturnDTO Publisher { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
    }

    public class PublisherToReturnDTO
    {
        public string Id { get; set; }
        public string Fullname { get; set; }
    }
}
