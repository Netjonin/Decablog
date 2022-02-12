using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models
{
    public class Stack
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Stack name should be between 2 and 10 characters in length")]
        public string Name { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 10, ErrorMessage = "Description should be between 10 and 150 characters in length")]
        public string Description { get; set; }
        public List<UserStack> UserStacks { get; set; }
        public Stack()
        {
            UserStacks = new List<UserStack>();
        }
    }
}
