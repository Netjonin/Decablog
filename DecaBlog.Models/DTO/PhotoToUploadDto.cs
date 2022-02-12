using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class PhotoToUploadDto
    {
        [Required]
        public IFormFile Photo { get; set; }
    }
}
