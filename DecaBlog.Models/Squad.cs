using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models
{
    public class Squad
    {      
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [StringLength(8, MinimumLength = 5, ErrorMessage = "Squad name should be between 5 and 8 characters in length")]
        public string Name { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 10, ErrorMessage = "Description should be between 10 and 150 characters in length")]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; } 
        public DateTime GradDate { get; set; }
        public bool IsGraduated { get; set; } 
        public List<UserSquad> UserSquads { get; set; }
        public Squad()
        {
            UserSquads = new List<UserSquad>();
        }

    }
}
