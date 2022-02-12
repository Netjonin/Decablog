using System.Collections.Generic;

namespace DecaBlog.Models.DTO
{
    public class UserMinInfoToReturnDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Role { get; set; }
        public string Stack { get; set; }
        public string Squad { get; set; }
        public string Photo { get; set; }
        public UserMinInfoToReturnDto()
        {
            Role = new List<string>();
        }
    }
}
