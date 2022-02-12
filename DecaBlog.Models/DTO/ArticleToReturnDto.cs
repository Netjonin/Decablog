using System;
using System.Collections.Generic;

namespace DecaBlog.Models.DTO
{
    public class ArticleToReturnDto
    {
        public Topic Topic { get; set; }
        public HashSet<string> Tags { get; set; } = new HashSet<string>();
        public List<Contributor> Contributors { get; set; } = new List<Contributor>();
        public List<SubTopic> Articles { get; set; }
    }

    public class Topic
    {
        public string TopicId { get; set; }
        public string TopicName { get; set; }
        public string Abstract { get; set; }
        public string Category { get; set; }
        public string PublicId { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime Date { get; set; }
    }

    public class SubTopic
    {
        public string SubId { get; set; }
        public string SubTopicName { get; set; }
        public string Text { get; set; }
        public Publisher Publisher { get; set; }
        public DateTime Date { get; set; }
    }

    public class Publisher
    {
        public string AuthorId { get; set; }
        public string FullName { get; set; }
    }
}
