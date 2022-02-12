using System;
using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class SquadToAddDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime GradDate { get; set; }
        public bool IsGraduated { get; set; }
    }
}
