using System.Collections.Generic;

namespace DecaBlog.Models.DTO
{
   public  class UserFullInfoDto
    { 
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public List<string> Role { get; set; } = new List<string>();
        public string Stack { get; set; }
        public string Squard { get; set; }
        public AddressDto Address { get; set; }
        public List<ArticleInfoDto> Articles { get; set; }
    }
}

