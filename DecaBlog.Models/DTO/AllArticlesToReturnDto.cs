using System.Collections.Generic;

namespace DecaBlog.Models.DTO
{
    public class AllArticlesToReturnDto
    {
        public string TopicId { get; set; }
        public string Topic { get; set; }
        public string Abstract { get; set; }
        public HashSet<string> Tags { get; set; } = new();
        public string Coverphotourl { get; set; }
        public AuthorDto Author { get; set; }
    }

    public class AuthorDto
    {
        public string AuthorId { get; set; }
        public string FullName { get; set; }
        public string AuthorPhotoUrl { get; set; }
        public string Stack { get; set; }
        public string Squad { get; set; }
    }
}