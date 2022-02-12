using DecaBlog.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DecaBlog.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> ForgotPassword(string email);
        Task<LoginResponseDto> Login(LoginDto model);
        Task<bool> ChangePassword(string Id, PasswordToUpdateDto model);
    }
}
