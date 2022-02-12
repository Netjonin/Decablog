using Microsoft.AspNetCore.Http;

namespace DecaBlog.Models.DTO
{
    public class ArticlePhoto
    {
        public IFormFile Photo { get; set; }
    }
}