using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models
{
    public class Notification : BaseEntity
    {
        [Required]
        public string ActivityId { get; set; }
        [Required]
        public string NoticeText { get; set; }
    }
}
