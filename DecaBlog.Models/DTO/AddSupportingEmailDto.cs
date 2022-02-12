using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecaBlog.Models.DTO
{
    public class AddSupportingEmailDto
    {
        [Required]
        [EmailAddress(ErrorMessage ="Please provide a valid email address")]
        public string Email { get; set; }
    }
}
