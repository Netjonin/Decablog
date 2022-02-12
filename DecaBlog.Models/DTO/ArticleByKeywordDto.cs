namespace DecaBlog.Models.DTO
{
    public class ArticleByKeywordDto
    {
        public string ArticleId { get; set; }
        public string SubTopic { get; set; }
        public string KeyWord { get; set; }
        public Contributor Contributor { get; set; }
        public string ArticleText { get; set; }
    }
    public class Contributor
    {
        public string FullName { get; set; }
        public string Id { get; set; }
    }
}
