using System;
using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models
{
    public class SupportingEmail
    {
        [Key]
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; }
        // navigation props
        public User User { get; set; }
    }
}

