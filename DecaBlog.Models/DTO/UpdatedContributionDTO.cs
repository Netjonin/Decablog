namespace DecaBlog.Models.DTO
{
    public class UpdatedContributionDTO
    {
        public string FullName { get; set; }
        public string CategoryId { get; set; }
        public string Abstract { get; set; }
        public string ArticleTopicId { get; set; }
        public string SubTopic { get; set; }
        public string Keywords { get; set; }
        public string DateUpdated { get; set; }
    }
}