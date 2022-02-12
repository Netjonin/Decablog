using Microsoft.AspNetCore.Identity;

namespace DecaBlog.Models.DTO
{
    public class RegisterResponseDto
    {
        public string Tag { get; set; }
        public string Key { get; set; }
        public string Message { get; set; }
        public IdentityResult ErrorResult { get; set; }
        public UserToReturnDto User { get; set; }
        public string Token { get; set; }
    }
}
