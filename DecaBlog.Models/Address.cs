using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models
{
    public class Address
    {
        [Key]
        public string UserId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Street { get; set; }
        [Required]
        [MaxLength(150)]
        public string State { get; set; }
        [Required]
        [MaxLength(150)]
        public string Country { get; set; }
        // navigation props
        public User User { get; set; }
    }
}