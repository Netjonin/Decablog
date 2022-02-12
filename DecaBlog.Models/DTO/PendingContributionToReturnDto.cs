using System;

namespace DecaBlog.Models.DTO
{
    public class PendingContributionToReturnDto
    {
        public string ArticleId { get; set; }
        public string ContributorId { get; set; }
        public string TopicId { get; set; }
        public string Topic { get; set; }
        public string Subtopic { get; set; }
        public string Keywords { get; set; }
        public string ArticleText { get; set; }
        public DateTime DateUpdated { get; set; } = DateTime.Now;
    }
}
