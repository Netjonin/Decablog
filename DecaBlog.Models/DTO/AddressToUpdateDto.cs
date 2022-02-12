using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecaBlog.Models.DTO
{
    public class AddressToUpdateDto
    {
        public string Street { get; set; }
        [Required]
        [MaxLength(150)]
        public string State { get; set; }
        [Required]
        [MaxLength(150)]
        public string Country { get; set; }
    }
}
