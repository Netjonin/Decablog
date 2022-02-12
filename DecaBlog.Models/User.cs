using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DecaBlog.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Must be between 3 and 15")]
        public string LastName { get; set; } 
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Must be between 3 and 15")]
        public string FirstName { get; set; }
        public string PhotoUrl { get; set; }
        public string PhotoPublicId { get; set; }
        public string InvitationToken { get; set; }
        public string Gender { get; set; }     
        public bool IsActive { get; set; }
        // navigation props
        public Address Address { get; set; }
        public UserComment UserComment { get; set; }
        public List<Photo> Photos { get; set; }
        public List<Article> Articles { get; set; }
        public List<UserSquad> UserSquads { get; set; }
        public List<UserStack> UserStacks { get; set; }
        public SupportingEmail SupportingEmails { get; set; }
        public User()
        {
            Articles = new List<Article>();
            Photos = new List<Photo>();
            UserStacks = new List<UserStack>();
            UserSquads = new List<UserSquad>();
        }
    }
}
