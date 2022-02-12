using System;

namespace DecaBlog.Models.DTO
{
    public class SquadMinInfoToReturnDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime GradDate { get; set; }
        public bool IsGraduated { get; set; }
    }
}
