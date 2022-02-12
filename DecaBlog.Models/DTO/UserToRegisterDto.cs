using System.ComponentModel.DataAnnotations;

namespace DecaBlog.Models.DTO
{
    public class UserToRegisterDto
    {
        [Required(ErrorMessage = "First name is a required field.")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Must be between 3 and 15")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is a required field.")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Must be between 3 and 15")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Gender is a required field.")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Email is a required field.")]
        [RegularExpression("^[A-Za-z0-9._%+-]+@decagon.dev$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Password field cannot be empty")]
        [MinLength(6, ErrorMessage = "Password cannot be less than 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Does not match password")]
        public string ConfirmPassword { get; set; }
        public string StackId { get; set; }
        public string SquadId { get; set; }
    }
}
