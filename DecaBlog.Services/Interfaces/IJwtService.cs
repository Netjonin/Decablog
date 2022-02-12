using DecaBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecaBlog.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJWTToken(User user, IEnumerable<string> userRoles);
        string GenerateInvitationToken(string userId, string email);
    }
}
